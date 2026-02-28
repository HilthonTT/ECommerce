using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.ServiceDefaults.Clients.ChatCompletion;

public static class TestCachingChatClientBuilderExtensions
{
    public static ChatClientBuilder UseCachingForTest(this ChatClientBuilder builder)
    {
        return builder.Use((client, serviceProvider) => {
            var cacheDir = serviceProvider.GetRequiredService<IConfiguration>()["E2E_TEST_CHAT_COMPLETION_CACHE_DIR"];
            if (!string.IsNullOrEmpty(cacheDir))
            {
                builder.UseDistributedCache(new DiskCache(cacheDir));
            }
            return client;
        });
    }

    /// <summary>
    /// An <see cref="IDistributedCache"/> that stores data in the filesystem.
    /// </summary>
    private sealed class DiskCache(string cacheDir) : IDistributedCache
    {
        public byte[]? Get(string key)
        {
            var path = FilePath(key);
            return File.Exists(path) ? File.ReadAllBytes(path) : null;
        }

        public async Task<byte[]?> GetAsync(string key, CancellationToken token = default)
        {
            var path = FilePath(key);
            return File.Exists(path) ? await File.ReadAllBytesAsync(path, token) : null;
        }

        public void Refresh(string key)
        {
        }

        public Task RefreshAsync(string key, CancellationToken token = default)
            => Task.CompletedTask;

        public void Remove(string key)
            => File.Delete(FilePath(key));

        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            Remove(key);
            return Task.CompletedTask;
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            var path = FilePath(key);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllBytes(path, value);
        }

        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            var path = FilePath(key);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            await File.WriteAllBytesAsync(path, value, token);
        }

        private string FilePath(string key)
            => Path.Combine(cacheDir, $"{key}.json");
    }
}
