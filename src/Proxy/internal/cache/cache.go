package cache

import (
	"encoding/gob"
	"io"
	"os"
	"sync"
	"time"
)

type Item struct {
	Value       any
	Expiration  int64
	Created     time.Time
	LastAccess  time.Time
	AccessCount int
}

func (item Item) IsExpired() bool {
	if item.Expiration == 0 {
		return false
	}
	return time.Now().UnixNano() > item.Expiration
}

type Cache struct {
	items           map[string]Item
	mu              sync.RWMutex
	cleanupInterval time.Duration
	maxItems        int
	evictionPolicy  EvictionPolicy
	stopCleanup     chan bool
	onEvicted       func(string, interface{})
	stats           Stats
}

// EvictionPolicy determines how items are evicted when the cache is full
type EvictionPolicy int

const (
	// LRU evicts the least recently used items
	LRU EvictionPolicy = iota
	// LFU evicts the least frequently used items
	LFU
	// FIFO evicts the oldest items
	FIFO
)

// Stats tracks cache performance metrics
type Stats struct {
	Hits       int64
	Misses     int64
	Evictions  int64
	TotalItems int64
}

// Options configures the cache
type Options struct {
	CleanupInterval time.Duration
	MaxItems        int
	EvictionPolicy  EvictionPolicy
	OnEvicted       func(string, any)
}

func DefaultOptions() Options {
	return Options{
		CleanupInterval: 5 * time.Minute,
		MaxItems:        0, // No limit
		EvictionPolicy:  LRU,
		OnEvicted:       nil,
	}
}

// NewCache creates a new cache with the given options
func NewCache(options Options) *Cache {
	cache := &Cache{
		items:           make(map[string]Item),
		cleanupInterval: options.CleanupInterval,
		maxItems:        options.MaxItems,
		evictionPolicy:  options.EvictionPolicy,
		stopCleanup:     make(chan bool),
		onEvicted:       options.OnEvicted,
	}

	// Start the cleanup goroutine
	go cache.startCleanupTimer()

	return cache
}

// startCleanupTimer starts the timer for cleanup
func (c *Cache) startCleanupTimer() {
	ticker := time.NewTicker(c.cleanupInterval)
	defer ticker.Stop()

	for {
		select {
		case <-ticker.C:
			c.cleanup()
		case <-c.stopCleanup:
			return
		}
	}
}

func (c *Cache) cleanup() {
	c.mu.Lock()
	defer c.mu.Unlock()

	now := time.Now().UnixNano()
	for key, item := range c.items {
		if item.Expiration > 0 && now > item.Expiration {
			c.deleteItem(key)
		}
	}
}

func (c *Cache) evict() {
	if c.maxItems <= 0 || len(c.items) < c.maxItems {
		return
	}

	var keyToEvict string
	var oldestTime time.Time
	var lowestCount int

	switch c.evictionPolicy {
	case LRU:
		// Find the least recently accessed item
		for k, item := range c.items {
			if keyToEvict == "" || item.LastAccess.Before(oldestTime) {
				keyToEvict = k
				oldestTime = item.LastAccess
			}
		}
	case LFU:
		// Find the least frequently accessed item
		for k, item := range c.items {
			if keyToEvict == "" || item.AccessCount < lowestCount {
				keyToEvict = k
				lowestCount = item.AccessCount
			}
		}
	case FIFO:
		// Find the oldest item
		for k, item := range c.items {
			if keyToEvict == "" || item.Created.Before(oldestTime) {
				keyToEvict = k
				oldestTime = item.Created
			}
		}
	}

	if keyToEvict != "" {
		c.deleteItem(keyToEvict)
		c.stats.Evictions++
	}
}

func (c *Cache) deleteItem(key string) {
	if c.onEvicted != nil {
		if item, found := c.items[key]; found {
			c.onEvicted(key, item.Value)
		}
	}

	delete(c.items, key)
}

func (c *Cache) Set(key string, value any, expiration time.Duration) {
	c.mu.Lock()
	defer c.mu.Unlock()

	// Check if we need to evict an item
	_, found := c.items[key]
	if c.maxItems > 0 && len(c.items) >= c.maxItems && !found {
		c.evict()
	}

	var exp int64
	if expiration > 0 {
		exp = time.Now().Add(expiration).UnixNano()
	}

	now := time.Now()
	c.items[key] = Item{
		Value:       value,
		Expiration:  exp,
		Created:     now,
		LastAccess:  now,
		AccessCount: 0,
	}

	c.stats.TotalItems++
}

func (c *Cache) Get(key string) (any, bool) {
	c.mu.Lock()
	defer c.mu.Unlock()

	item, found := c.items[key]
	if !found {
		c.stats.Misses++
		return nil, false
	}

	if item.IsExpired() {
		c.deleteItem(key)
		c.stats.Misses++
		return nil, false
	}

	item.LastAccess = time.Now()
	item.AccessCount++
	c.items[key] = item

	c.stats.Hits++

	return item.Value, true
}

func (c *Cache) GetWithExpiration(key string) (any, time.Time, bool) {
	c.mu.Lock()
	defer c.mu.Unlock()

	item, found := c.items[key]
	if !found {
		c.stats.Misses++
		return nil, time.Time{}, false
	}

	// Check if the item has expired
	if item.IsExpired() {
		c.deleteItem(key)
		c.stats.Misses++
		return nil, time.Time{}, false
	}

	// Update access stats
	item.LastAccess = time.Now()
	item.AccessCount++
	c.items[key] = item

	c.stats.Hits++

	var expiration time.Time
	if item.Expiration > 0 {
		expiration = time.Unix(0, item.Expiration)
	}

	return item.Value, expiration, true
}

func (c *Cache) Delete(key string) {
	c.mu.Lock()
	defer c.mu.Unlock()

	c.deleteItem(key)
}

func (c *Cache) Flush() {
	c.mu.Lock()
	defer c.mu.Unlock()

	c.items = make(map[string]Item)
	c.stats = Stats{}
}

func (c *Cache) Close() {
	c.stopCleanup <- true
}

func (c *Cache) Count() int {
	c.mu.RLock()
	defer c.mu.RUnlock()

	return len(c.items)
}

func (c *Cache) GetStats() Stats {
	c.mu.RLock()
	defer c.mu.RUnlock()

	return c.stats
}

func (c *Cache) SaveToFile(filename string) error {
	c.mu.RLock()
	defer c.mu.RUnlock()

	file, err := os.Create(filename)
	if err != nil {
		return err
	}
	defer file.Close()

	return c.saveToWriter(file)
}

// LoadFromFile loads the cache from a file
func (c *Cache) LoadFromFile(filename string) error {
	c.mu.Lock()
	defer c.mu.Unlock()

	file, err := os.Open(filename)
	if err != nil {
		return err
	}
	defer file.Close()

	return c.loadFromReader(file)
}

// saveToWriter encodes the cache to a writer
func (c *Cache) saveToWriter(w io.Writer) error {
	enc := gob.NewEncoder(w)

	// Only save unexpired items
	now := time.Now().UnixNano()
	items := make(map[string]Item)

	for k, v := range c.items {
		if v.Expiration == 0 || v.Expiration > now {
			items[k] = v
		}
	}

	return enc.Encode(items)
}

func (c *Cache) loadFromReader(r io.Reader) error {
	dec := gob.NewDecoder(r)
	items := make(map[string]Item)

	if err := dec.Decode(&items); err != nil {
		return err
	}

	// Only load unexpired items
	now := time.Now().UnixNano()
	for k, v := range items {
		if v.Expiration == 0 || v.Expiration > now {
			c.items[k] = v
		}
	}

	return nil
}
