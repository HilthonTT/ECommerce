package main

import (
	"context"
	"log"
	"net/http"
	"os"
	"time"

	"github.com/hilthontt/ecommerce/proxy/internal/balancer"
	"github.com/hilthontt/ecommerce/proxy/internal/profiler"
)

func main() {
	profileDir := "/var/log/ecommerce/profiles"
	os.MkdirAll(profileDir, 0755)

	profiler := profiler.NewAdaptiveProfiler(profileDir)
	profiler.Start(context.Background())

	backends := []string{
		"http://localhost:8081",
		"http://localhost:8082",
		"http://localhost:8083",
	}

	lb := balancer.NewLoadBalancer(backends, 30*time.Second, 3)
	server := http.Server{
		Addr:    ":8080",
		Handler: lb,
	}

	log.Printf("Starting load balancer on :8080")
	log.Fatal(server.ListenAndServe())
}
