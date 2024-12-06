using GithubRepositoryAnalyzer.Kernel.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace GithubRepositoryAnalyzer.Kernel.Extensions;

public static class ServiceProviderX
{
    public static void AddRedisCacheStorage<T>(
        this IServiceCollection services)
    {
        services.AddScoped<ICacheStorage<T>>(sp =>
        {
            var distributedCache = sp.GetRequiredService<IDistributedCache>();
            var connectionMultiplexer = sp.GetRequiredService<IConnectionMultiplexer>();

            return new RedisCacheStorage<T>(distributedCache, connectionMultiplexer);
        });
    }
}