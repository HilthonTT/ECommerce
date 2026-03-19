package balancer

import (
	"net/http/httputil"
	"net/url"
	"sync"
)

type Backend struct {
	URL          *url.URL
	Alive        bool
	ReverseProxy *httputil.ReverseProxy
	mux          sync.RWMutex
	failCount    int
}

func (b *Backend) SetAlive(alive bool) {
	b.mux.Lock()
	b.Alive = alive
	if alive {
		b.failCount = 0
	}
	b.mux.Unlock()
}

func (b *Backend) IsAlive() bool {
	b.mux.RLock()
	alive := b.Alive
	b.mux.RUnlock()
	return alive
}

func (b *Backend) IncreaseFailCount() int {
	b.mux.Lock()
	b.failCount++
	count := b.failCount
	b.mux.Unlock()
	return count
}

func (b *Backend) ResetFailCount() {
	b.mux.Lock()
	b.failCount = 0
	b.mux.Unlock()
}
