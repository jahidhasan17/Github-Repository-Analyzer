using StackExchange.Redis;

namespace GithubRepositoryAnalyzer.EventMessaging.Worker.Services;

public static class ServiceProviderX
{
    public static void AddRedisCacheStorage<T>(
        this IServiceCollection services)
    {
        services.AddScoped<ICacheStorage<T>>(sp =>
        {
            var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();

            return new RedisCacheStorage<T>(multiplexer);
        });
    }
}