using GithubRepositoryAnalyzer.Services;

namespace GithubRepositoryAnalyzer;

public static class ServiceCollectionX
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
    }
}