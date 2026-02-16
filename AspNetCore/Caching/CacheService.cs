using Microsoft.Extensions.Caching.Memory;

// Based on: DNTFrameworkCore

namespace Brupper.AspNetCore.Caching;

/// <summary> Encapsulates IMemoryCache functionality. </summary>
internal sealed class MemoryCacheService(IMemoryCache memoryCache) : ICacheService
{
    private readonly IMemoryCache memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

    /// <summary>  Gets the key's value from the cache. </summary>
    public T Get<T>(string cacheKey) => memoryCache.Get<T>(cacheKey);

    /// <summary> Tries to get the key's value from the cache. </summary>
    public bool TryGetValue<T>(string cacheKey, out T result) => memoryCache.TryGetValue(cacheKey, out result);

    /// <summary> Adds a key-value to the cache. It will use the factory method to get the value and then inserts it. </summary>
    public void Add<T>(string cacheKey, Func<T> factory, DateTimeOffset absoluteExpiration, int size = 1)
        => memoryCache.Set(cacheKey, factory(), new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = absoluteExpiration,
            Size = size // the size limit is the count of entries
        });

    /// <summary> Adds a key-value to the cache. It will use the factory method to get the value and then inserts it. </summary>
    public Task AddAsync<T>(string cacheKey, Func<Task<T>> factory, DateTimeOffset absoluteExpiration, int size = 1)
        => memoryCache.GetOrCreateAsync(cacheKey, async e =>
        {
            e.SetAbsoluteExpiration(absoluteExpiration);
            return await factory();
        });

    /// <summary> Adds a key-value to the cache. </summary>
    public void Add<T>(string cacheKey, T value, DateTimeOffset absoluteExpiration, int size = 1)
    {
        memoryCache.Set(cacheKey, value, new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = absoluteExpiration,
            Size = size // the size limit is the count of entries
        });
    }

    /// <summary> Adds a key-value to the cache. </summary>
    public void Add<T>(string cacheKey, T value, int size = 1)
    {
        memoryCache.Set(cacheKey, value, new MemoryCacheEntryOptions
        {
            Size = size // the size limit is the count of entries
        });
    }

    /// <summary>
    /// A thread-safe way of working with memory cache. First tries to get the key's value from the cache.
    /// Otherwise it will use the factory method to get the value and then inserts it.
    /// </summary>
    public T GetOrAdd<T>(string cacheKey, Func<T> factory, DateTimeOffset absoluteExpiration, int size = 1)
    {
        // locks get and set internally
        if (memoryCache.TryGetValue<T>(cacheKey, out var result))
        {
            return result;
        }

        lock (TypeLock<T>.Lock)
        {
            if (memoryCache.TryGetValue(cacheKey, out result))
            {
                return result;
            }

            result = factory();
            memoryCache.Set(cacheKey, result, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = absoluteExpiration,
                Size = size // the size limit is the count of entries
            });

            return result;
        }
    }

    /// <summary>
    /// A thread-safe way of working with memory cache. First tries to get the key's value from the cache.
    /// Otherwise it will use the factory method to get the value and then inserts it.
    /// </summary>
    public Task<T> GetOrAddAsync<T>(string cacheKey, Func<Task<T>> factory, DateTimeOffset absoluteExpiration, int size = 1)
        => memoryCache.GetOrCreateAsync(cacheKey, async e =>
        {
            e.SetAbsoluteExpiration(absoluteExpiration);
            return await factory();
        });

    /// <summary> Removes the object associated with the given key. </summary>
    public void Remove(string cacheKey) => memoryCache.Remove(cacheKey);

    private static class TypeLock<T>
    {
        // ReSharper disable once StaticMemberInGenericType
        public static object Lock { get; } = new object();
    }
}
