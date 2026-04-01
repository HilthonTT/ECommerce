package cache

import (
	"bytes"
	"net/http"
	"time"
)

type ResponseCache struct {
	cache *ShardedCache
}

type CachedResponse struct {
	StatusCode int
	Headers    map[string]string
	Body       []byte
}

func NewResponseCache() *ResponseCache {
	options := DefaultOptions()
	options.CleanupInterval = 5 * time.Minute

	return &ResponseCache{
		cache: NewShardedCache(options, DefaultShards),
	}
}

func (rc *ResponseCache) Middleware(next http.Handler) http.Handler {
	return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		// Only cache GET requests
		if r.Method != http.MethodGet {
			next.ServeHTTP(w, r)
			return
		}

		// Create a cache key from the request
		key := r.URL.String()

		// Check if we have a cached response
		if cachedResp, found := rc.cache.Get(key); found {
			resp := cachedResp.(*CachedResponse)

			// Set headers
			for k, v := range resp.Headers {
				w.Header().Set(k, v)
			}

			// Write status code and body
			w.WriteHeader(resp.StatusCode)
			w.Write(resp.Body)
			return
		}

		// Create a response recorder
		rr := newResponseRecorder(w)

		if rr.Header().Get("X-Cache-Response") == "" {
			return
		}

		// Call the next handler
		next.ServeHTTP(rr, r)

		// Cache the response
		resp := &CachedResponse{
			StatusCode: rr.statusCode,
			Headers:    make(map[string]string),
			Body:       rr.body.Bytes(),
		}

		// Copy headers
		for k, v := range rr.Header() {
			if len(v) > 0 {
				resp.Headers[k] = v[0]
			}
		}

		// Store in cache with TTL
		rc.cache.Set(key, resp, 5*time.Minute)
	})
}

type responseRecorder struct {
	http.ResponseWriter
	statusCode int
	body       bytes.Buffer
}

func newResponseRecorder(w http.ResponseWriter) *responseRecorder {
	return &responseRecorder{
		ResponseWriter: w,
		statusCode:     http.StatusOK, // default if WriteHeader is never called
	}
}

func (rr *responseRecorder) WriteHeader(code int) {
	rr.statusCode = code
	rr.ResponseWriter.WriteHeader(code)
}

func (rr *responseRecorder) Write(b []byte) (int, error) {
	rr.body.Write(b)                  // capture into buffer
	return rr.ResponseWriter.Write(b) // also write to the real writer
}
