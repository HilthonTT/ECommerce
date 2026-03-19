package profiler

import (
	"context"
	"fmt"
	"os"
	"runtime"
	"runtime/pprof"
	"sync"
	"time"
)

type AdaptiveProfiler struct {
	// Configuration
	profileDir      string
	cpuThreshold    float64 // CPU threshold to trigger profiling (0-1)
	memThreshold    float64 // Memory threshold (0-1)
	minInterval     time.Duration
	profileDuration time.Duration

	// State
	lastProfile time.Time
	mutex       sync.Mutex
	isRunning   bool
}

func NewAdaptiveProfiler(profileDir string) *AdaptiveProfiler {
	return &AdaptiveProfiler{
		profileDir:      profileDir,
		cpuThreshold:    0.70, // Start profiling at 70% CPU
		memThreshold:    0.80, // Start profiling at 80% memory
		minInterval:     10 * time.Minute,
		profileDuration: 30 * time.Second,
		lastProfile:     time.Time{},
	}
}

func (p *AdaptiveProfiler) Start(ctx context.Context) {
	go p.monitor(ctx)
}

func (p *AdaptiveProfiler) monitor(ctx context.Context) {
	ticker := time.NewTicker(15 * time.Second)
	defer ticker.Stop()

	for {
		select {
		case <-ticker.C:
			p.checkAndProfile()
		case <-ctx.Done():
			return
		}
	}
}

func (p *AdaptiveProfiler) checkAndProfile() {
	p.mutex.Lock()
	defer p.mutex.Unlock()

	if p.isRunning {
		return
	}

	// Check if we've profiled recently
	if time.Since(p.lastProfile) < p.minInterval {
		return
	}

	var m runtime.MemStats
	runtime.ReadMemStats(&m)

	memUsage := float64(m.Alloc) / float64(m.Sys)

	// Use GOMAXPROCS as an approximation for available CPU
	cpus := runtime.GOMAXPROCS(0)
	var cpuUsage float64

	// Here you would calculate CPU usage
	// For a real implementation, you'd use something like:
	// - Read /proc/stat on Linux
	// - Use syscall.GetProcessTimes on Windows
	// For simplicity, we'll assume a function exists
	cpuUsage = getCPUUsage(cpus)

	// If thresholds are exceeded, profile
	if cpuUsage > p.cpuThreshold || memUsage > p.memThreshold {
		p.isRunning = true
		go p.captureProfiles()
	}
}

func getCPUUsage(cpus int) float64 {
	// Implementation depends on OS
	// Simple implementation that would need to be replaced
	return 0.5 // Placeholder
}

func (p *AdaptiveProfiler) captureProfiles() {
	timestamp := time.Now().Format("20060102-150405")

	// Capture CPU profile
	cpuFile, err := os.Create(fmt.Sprintf("%s/cpu-%s.pprof", p.profileDir, timestamp))
	if err != nil {
		// Log error and continue
		fmt.Printf("Error creating CPU profile: %v\n", err)
	} else {
		runtime.GC() // Run GC before profiling
		pprof.StartCPUProfile(cpuFile)
		time.Sleep(p.profileDuration) // Profile for N seconds
		pprof.StopCPUProfile()
		cpuFile.Close()
	}

	// Capture memory profile
	memFile, err := os.Create(fmt.Sprintf("%s/mem-%s.pprof", p.profileDir, timestamp))
	if err != nil {
		fmt.Printf("Error creating memory profile: %v\n", err)
	} else {
		runtime.GC() // Run GC before profiling
		if err := pprof.WriteHeapProfile(memFile); err != nil {
			fmt.Printf("Error writing memory profile: %v\n", err)
		}
		memFile.Close()
	}

	// Capture goroutine profile
	goroutineFile, err := os.Create(fmt.Sprintf("%s/goroutine-%s.pprof", p.profileDir, timestamp))
	if err != nil {
		fmt.Printf("Error creating goroutine profile: %v\n", err)
	} else {
		p := pprof.Lookup("goroutine")
		if p != nil {
			p.WriteTo(goroutineFile, 0)
		}
		goroutineFile.Close()
	}

	// Mark profiling complete
	p.mutex.Lock()
	p.lastProfile = time.Now()
	p.isRunning = false
	p.mutex.Unlock()
}
