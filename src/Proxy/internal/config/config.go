package config

import (
	"fmt"
	"time"

	"github.com/hilthontt/ecommerce/proxy/internal/balancer"
)

type Config struct {
	ListenAddr          string          `json:"listen_addr"`
	HealthCheckInterval time.Duration   `json:"health_check_interval"`
	MaxFailCount        int             `json:"max_fail_count"`
	Strategy            string          `json:"strategy"`
	Backends            []BackendConfig `json:"backends"`
}

type BackendConfig struct {
	URL    string `json:"url"`
	Weight int    `json:"weight"`
}

func ParseStrategyString(s string) (balancer.Strategy, error) {
	switch s {
	case "round_robin":
		return balancer.RoundRobin, nil
	case "least_connections":
		return balancer.LeastConnections, nil
	case "ip_hash":
		return balancer.IPHash, nil
	case "random":
		return balancer.Random, nil
	case "weighted_round_robin":
		return balancer.WeightedRoundRobin, nil
	default:
		return 0, fmt.Errorf("unknown strategy: %s", s)
	}
}
