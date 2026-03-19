package balancer

import (
	"context"
	"log"
	"net"
	"net/http"
	"net/http/httputil"
	"net/url"
	"sync"
	"time"
)

type LoadBalancer struct {
	backends            []*Backend
	current             int
	mux                 sync.Mutex
	healthCheckInterval time.Duration
	maxFailCount        int
}

func NewLoadBalancer(backendURLs []string, healthCheckInterval time.Duration, maxFailCount int) *LoadBalancer {
	backends := make([]*Backend, len(backendURLs))

	for i, rawURL := range backendURLs {
		url, err := url.Parse(rawURL)
		if err != nil {
			log.Fatal(err)
		}

		backends[i] = &Backend{
			URL:          url,
			Alive:        true,
			ReverseProxy: httputil.NewSingleHostReverseProxy(url),
		}

		// Configure error handler for passive health checks
		backends[i].ReverseProxy.ErrorHandler = func(w http.ResponseWriter, r *http.Request, err error) {
			backend := backends[i]
			failCount := backend.IncreaseFailCount()
			log.Printf("Backend %s request failed: %v, fail count: %d", backend.URL.Host, err, failCount)

			// Mark server as down if it fails too many times
			if failCount >= maxFailCount {
				log.Printf("Backend %s is marked as down due to too many failures", backend.URL.Host)
				backend.SetAlive(false)
			}

			// Find a new backend for this request
			lb := r.Context().Value("loadbalancer").(*LoadBalancer)
			if newBackend := lb.NextBackend(); newBackend != nil {
				log.Printf("Retrying request on backend %s", newBackend.URL.Host)
				newBackend.ReverseProxy.ServeHTTP(w, r)
				return
			}

			// If all backends are down
			http.Error(w, "Service Unavailable", http.StatusServiceUnavailable)
		}
	}

	lb := &LoadBalancer{
		backends:            backends,
		healthCheckInterval: healthCheckInterval,
		maxFailCount:        maxFailCount,
	}

	// Start active health checks
	go lb.healthCheck()

	return lb
}

func (lb *LoadBalancer) NextBackend() *Backend {
	lb.mux.Lock()
	defer lb.mux.Unlock()

	initialIndex := lb.current

	for {
		lb.current = (lb.current + 1) % len(lb.backends)
		if lb.backends[lb.current].IsAlive() {
			return lb.backends[lb.current]
		}

		// If we've checked all backends and none are alive, or we've come full circle
		if lb.current == initialIndex {
			return nil
		}
	}
}

func (lb *LoadBalancer) ServeHTTP(w http.ResponseWriter, r *http.Request) {
	// Store load balancer in context for error handler
	ctx := context.WithValue(r.Context(), "loadbalancer", lb)
	r = r.WithContext(ctx)

	backend := lb.NextBackend()
	if backend == nil {
		http.Error(w, "No available backends", http.StatusServiceUnavailable)
		return
	}

	log.Printf("Forwarding request to: %s", backend.URL.Host)
	backend.ReverseProxy.ServeHTTP(w, r)

	// Reset fail count on successful request (passive health check)
	backend.ResetFailCount()
}

func isBackendAlive(u *url.URL) bool {
	timeout := 2 * time.Second
	conn, err := net.DialTimeout("tcp", u.Host, timeout)
	if err != nil {
		log.Printf("Health check failed for %s: %v", u.Host, err)
		return false
	}
	defer conn.Close()
	return true
}

func (lb *LoadBalancer) healthCheck() {
	ticker := time.NewTicker(lb.healthCheckInterval)
	defer ticker.Stop()

	for range ticker.C {
		log.Println("Starting health check...")
		for _, backend := range lb.backends {
			alive := isBackendAlive(backend.URL)
			backend.SetAlive(alive)
			status := "up"
			if !alive {
				status = "down"
			}
			log.Printf("Backend %s status: %s", backend.URL.Host, status)
		}
		log.Println("Health check completed")
	}
}
