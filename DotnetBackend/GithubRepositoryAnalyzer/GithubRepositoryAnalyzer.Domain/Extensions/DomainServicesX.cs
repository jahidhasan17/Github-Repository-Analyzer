using GithubRepositoryAnalyzer.Domain.Context;
using GithubRepositoryAnalyzer.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GithubRepositoryAnalyzer.Domain.Extensions;

public static class DomainServicesX
{
    public static void AddDatabaseServices(this IServiceCollection services)
    {
        services.AddDbContext<ConfigContext>();
        services.AddScoped<IDatabaseMigrateService, DatabaseMigrateService>();
    }
}