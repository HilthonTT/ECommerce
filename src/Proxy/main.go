package main

import (
	"context"
	"encoding/json"
	"errors"
	"flag"
	"fmt"
	"log"
	"net/http"
	"net/http/httputil"
	"net/url"
	"os"
	"os/signal"
	"syscall"
	"time"

	"github.com/hilthontt/ecommerce/proxy/internal/balancer"
	"github.com/hilthontt/ecommerce/proxy/internal/config"
	"github.com/hilthontt/ecommerce/proxy/internal/profiler"
	"github.com/prometheus/client_golang/prometheus/promhttp"
)

const profileDir = "/var/log/ecommerce/profiles"

func main() {
	configPath := flag.String("config", "", "Path to configuration file")
	listenAddr := flag.String("listen", ":8080", "Address to listen on")
	strategyStr := flag.String("strategy", "round_robin", "Load balancing strategy")
	healthCheckInterval := flag.Duration("health-check-interval", 30*time.Second, "Health check interval")
	maxFailCount := flag.Int("max-fail-count", 3, "Maximum failure count before marking backend as down")

	flag.Parse()

	//  Graceful shutdown context
	ctx, stop := signal.NotifyContext(context.Background(), os.Interrupt, syscall.SIGTERM)
	defer stop()

	// Profiler
	if err := os.MkdirAll(profileDir, 0755); err != nil {
		log.Fatalf("Failed to create profile directory: %v", err)
	}

	prof := profiler.NewAdaptiveProfiler(profileDir)
	prof.Start(ctx)

	// Config
	cfg, err := loadConfig(*configPath, *listenAddr, *strategyStr, *healthCheckInterval, *maxFailCount)
	if err != nil {
		log.Fatalf("Configuration error: %v", err)
	}

	strategy, err := config.ParseStrategyString(cfg.Strategy)
	if err != nil {
		log.Fatalf("Invalid strategy: %v", err)
	}

	backendURLs := make([]string, len(cfg.Backends))
	weights := make([]int, len(cfg.Backends))

	for i, b := range cfg.Backends {
		backendURLs[i] = b.URL
		weights[i] = b.Weight
	}

	// Metrics
	metrics := balancer.NewMetrics("loadbalancer")

	lb := balancer.NewLoadBalancer(
		backendURLs,
		weights,
		cfg.HealthCheckInterval,
		cfg.MaxFailCount,
		strategy,
	)
	lb.Metrics = metrics

	// Create a mux to handle both load balancing and metrics
	mux := http.NewServeMux()
	mux.Handle("/", lb)
	mux.Handle("/metrics", promhttp.Handler())
	mux.HandleFunc("/admin/reload", func(w http.ResponseWriter, r *http.Request) {
		if r.Method != http.MethodPost {
			http.Error(w, "Method not allowed", http.StatusMethodNotAllowed)
			return
		}

		if err := reloadConfiguration(lb, *configPath); err != nil {
			http.Error(w, err.Error(), http.StatusInternalServerError)
			return
		}

		w.WriteHeader(http.StatusOK)
		w.Write([]byte("Configuration reloaded successfully"))
	})

	server := &http.Server{
		Addr:         cfg.ListenAddr,
		Handler:      mux,
		ReadTimeout:  15 * time.Second,
		WriteTimeout: 30 * time.Second,
		IdleTimeout:  60 * time.Second,
	}

	errCh := make(chan error, 1)
	go func() {
		log.Printf("Starting load balancer on %s with strategy: %s", cfg.ListenAddr, cfg.Strategy)
		log.Printf("Metrics available at %s/metrics", cfg.ListenAddr)
		if err := server.ListenAndServe(); !errors.Is(err, http.ErrServerClosed) {
			errCh <- err
		}
		close(errCh)
	}()

	select {
	case err := <-errCh:
		log.Fatalf("Server error: %v", err)
	case <-ctx.Done():
		log.Println("Shutting down gracefully...")
	}

	shutdownCtx, cancel := context.WithTimeout(context.Background(), 15*time.Second)
	defer cancel()

	if err := server.Shutdown(shutdownCtx); err != nil {
		log.Fatalf("Forced shutdown: %v", err)
	}

	log.Println("Server stopped")
}

func loadConfig(
	path, listenAddr, strategy string,
	healthCheckInterval time.Duration,
	maxFailCount int,
) (config.Config, error) {
	if path != "" {
		data, err := os.ReadFile(path)
		if err != nil {
			return config.Config{}, err
		}

		var cfg config.Config
		if err := json.Unmarshal(data, &cfg); err != nil {
			return config.Config{}, err
		}
		return cfg, nil
	}

	return config.Config{
		ListenAddr:          listenAddr,
		HealthCheckInterval: healthCheckInterval,
		MaxFailCount:        maxFailCount,
		Strategy:            strategy,
		Backends: []config.BackendConfig{
			{URL: "http://localhost:8081", Weight: 1},
			{URL: "http://localhost:8082", Weight: 1},
			{URL: "http://localhost:8083", Weight: 1},
		},
	}, nil
}

func reloadConfiguration(lb *balancer.LoadBalancer, configPath string) error {
	if configPath == "" {
		return fmt.Errorf("no config file provided")
	}

	data, err := os.ReadFile(configPath)
	if err != nil {
		return fmt.Errorf("error reading config file: %v", err)
	}

	var cfg config.Config
	if err := json.Unmarshal(data, &cfg); err != nil {
		return fmt.Errorf("error parsing config file: %v", err)
	}

	// Parse strategy
	strategy, err := config.ParseStrategyString(cfg.Strategy)
	if err != nil {
		return fmt.Errorf("invalid strategy: %v", err)
	}

	// Extract backends and weights
	backendURLs := make([]string, len(cfg.Backends))
	weights := make([]int, len(cfg.Backends))

	for i, backend := range cfg.Backends {
		backendURLs[i] = backend.URL
		weights[i] = backend.Weight
	}

	lb.Mux.Lock()
	lb.HealthCheckInterval = cfg.HealthCheckInterval
	lb.MaxFailCount = cfg.MaxFailCount
	lb.Strategy = strategy

	// Update backends (keep the existing ones if they're still in the config)
	oldBackends := lb.Backends
	lb.Backends = make([]*balancer.Backend, len(cfg.Backends))

	for i, backendUrl := range backendURLs {
		// Check if this backend already exists
		found := false
		for _, oldBackend := range oldBackends {
			if oldBackend.URL.String() == backendUrl {
				// Keep the existing backend but update its weight
				lb.Backends[i] = oldBackend
				oldBackend.Weight = weights[i]
				found = true
				break
			}
		}
		if !found {
			parsedURL, _ := url.Parse(backendUrl) // Error already checked earlier
			lb.Backends[i] = &balancer.Backend{
				URL:          parsedURL,
				Alive:        true, // Assume alive until health check
				ReverseProxy: httputil.NewSingleHostReverseProxy(parsedURL),
				Weight:       weights[i],
			}
		}
	}

	lb.Mux.Unlock()
	log.Printf("Configuration reloaded with %d backends and strategy: %s", len(lb.Backends), cfg.Strategy)

	return nil
}
