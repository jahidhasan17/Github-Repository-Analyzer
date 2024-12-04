using System.Text.Json;
using StackExchange.Redis;

namespace GithubRepositoryAnalyzer.EventMessaging.Worker.Services;

public class RedisCacheStorage<TValue>(IConnectionMultiplexer connectionMultiplexer) : ICacheStorage<TValue>
{
    private readonly IDatabase database = connectionMultiplexer.GetDatabase();
    private static readonly SpecificKeyWriteLock @lock = new();
    
    public bool TryGetValue(string key, out TValue value)
    {
        var redisValue = database.StringGet(key);
        
        if (redisValue.HasValue)
        {
            value = JsonSerializer.Deserialize<TValue>(redisValue);
            return true;
        }
        
        value = default;
        
        return false;
    }

    public void AddOrUpdate(string key, TValue value)
    {
        var semaphore = @lock.Lock(key);
        try
        {
            database.StringSet(key, JsonSerializer.Serialize(value));
        }
        finally
        {
            @lock.Unlock(key, semaphore);
        }
    }

    public TValue GetOrAdd(string key, Func<TValue> initializationFunc)
    {
        if (TryGetValue(key, out var value))
        {
            return value;
        }
        
        var semaphore = @lock.Lock(key);
        try
        {
            if (TryGetValue(key, out var internalValue))
            {
                return internalValue;
            }

            var retrievedFromProviderValue = initializationFunc.Invoke();
            
            database.StringSet(key, JsonSerializer.Serialize(retrievedFromProviderValue));

            return retrievedFromProviderValue;
        }
        finally
        {
            @lock.Unlock(key, semaphore);
        }
    }
}