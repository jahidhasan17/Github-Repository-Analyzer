using GithubRepositoryAnalyzer.Config.Security;
using GithubRepositoryAnalyzer.Services;
using GithubRepositoryAnalyzer.Services.GithubUserSearchService;
using GithubRepositoryAnalyzer.Services.RepositorySearchService;
using GithubRepositoryAnalyzer.Services.RepositoryTimelineSearchService;

namespace GithubRepositoryAnalyzer;

public static class ServiceCollectionX
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IGithubUserSearchService, GithubUserSearchService>();
        services.AddScoped<IRepositorySearchService, RepositorySearchService>();
        services.AddScoped<IRepositoryTimelineSearchService, RepositoryTimelineSearchService>();
    }
}