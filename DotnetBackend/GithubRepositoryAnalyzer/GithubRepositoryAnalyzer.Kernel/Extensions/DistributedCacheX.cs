using System.Text.Json;
using GithubRepositoryAnalyzer.Kernel.JsonConverters;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace GithubRepositoryAnalyzer.Kernel.Extensions;

public static class DistributedCacheX
{
    public static bool TryGetRecord<T>(this IDistributedCache cache, string key, out T value)
    {
        var json = cache.GetString(key);
        value = default;

        if (string.IsNullOrEmpty(json))
        {
            return false;
        }

        try
        {
            value = JsonSerializer.Deserialize<T>(json, GetJsonSerializerOptions<T>());
        }
        catch
        {
            return false;
        }

        return true;
    }
    
    public static bool TryGetRecord<T>(this IDatabase database, string key, out T value)
    {
        var json = database.StringGet(key);
        value = default;

        if (string.IsNullOrEmpty(json))
        {
            return false;
        }

        try
        {
            value = JsonSerializer.Deserialize<T>(json, GetJsonSerializerOptions<T>());
        }
        catch
        {
            return false;
        }

        return true;
    }
    
    public static void SetRecord<T>(this IDistributedCache cache, string key, T value)
    {
        SetRecord(cache, key, value, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(2),
        });
    }
    
    private static void SetRecord<T>(
        this IDistributedCache cache,
        string key,
        T value,
        DistributedCacheEntryOptions options)
    {
        var jsonObject = JsonSerializer.Serialize(value, GetJsonSerializerOptions<T>());

        cache.SetString(key, jsonObject, options);
    }
    
    private static JsonSerializerOptions GetJsonSerializerOptions<T>()
    {
        var jsonSerializerOptions = new JsonSerializerOptions();

        var type = typeof(T).IsGenericType ? typeof(T).GetGenericArguments().First() : typeof(T);

        if (type.IsEnum)
        {
            jsonSerializerOptions.Converters.Add(new EnumConverter<T>());
        }

        if (typeof(T) == typeof(bool))
        {
            jsonSerializerOptions.Converters.Add(new BooleanConverter());
        }

        if (typeof(T) == typeof(int))
        {
            jsonSerializerOptions.Converters.Add(new IntConverter());
        }

        return jsonSerializerOptions;
    }
}