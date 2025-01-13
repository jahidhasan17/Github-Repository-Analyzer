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
    private IDatabase database => connectionMultiplexer.GetDatabase();
    private IServer server => connectionMultiplexer.GetServer(connectionMultiplexer.GetEndPoints().First());
    private static readonly SpecificKeyWriteLock @lock = new();
    
    public bool TryGetValue(string key, out TValue value) => database.TryGetRecord(key, out value);
    
    public bool AnyKeysWithPattern(string pattern)
    {
        var keys = server.Keys(pattern: pattern, pageSize: 1);
        
        return keys.Any();
    }

    public void AddOrUpdate(string key, TValue value)
    {
        var semaphore = @lock.Lock(key);
        try
        {
            database.SetRecord(key, value);
        }
        finally
        {
            @lock.Unlock(key, semaphore);
        }
    }
    
    public IEnumerable<TValue> GetValuesByPattern(string pattern)
    {
        // Use KEYS command to find matching keys
        var keys = server.Keys(pattern: pattern).ToList();
        var results = new List<TValue>();

        foreach (var key in keys)
        {
            if (database.TryGetRecord(key, out TValue value))
            {
                results.Add(value);
            }
            /*if (database.TryGetRecord(key.ToString(), out TValue value))
            {
                results.Add(value);
            }*/
        }

        return results;
    }
}