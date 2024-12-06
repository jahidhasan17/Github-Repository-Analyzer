using System.Text.Json;
using GithubRepositoryAnalyzer.Kernel.Extensions;
using GithubRepositoryAnalyzer.Kernel.Lock;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace GithubRepositoryAnalyzer.Kernel.Cache;


public class RedisCacheStorage<TValue>(
    IDistributedCache distributedCache,
    IConnectionMultiplexer connectionMultiplexer) : ICacheStorage<TValue>
{
    private static readonly SpecificKeyWriteLock @lock = new();
    
    public bool TryGetValue(string key, out TValue value) => distributedCache.TryGetRecord(key, out value);

    public void AddOrUpdate(string key, TValue value)
    {
        var semaphore = @lock.Lock(key);
        try
        {
            distributedCache.SetRecord(key, value);
        }
        finally
        {
            @lock.Unlock(key, semaphore);
        }
    }
    
    public IEnumerable<TValue> GetValuesByPattern(string pattern)
    {
        var database = connectionMultiplexer.GetDatabase();
        var endpoints = connectionMultiplexer.GetEndPoints();
        var server = connectionMultiplexer.GetServer(endpoints.First());

        // Use KEYS command to find matching keys
        var keys = server.Keys(pattern: pattern).ToList();
        var results = new List<TValue>();

        foreach (var key in keys)
        {
            if (database.TryGetRecord(key, out TValue value))
            {
                results.Add(value);
            }
        }

        return results;
    }
}