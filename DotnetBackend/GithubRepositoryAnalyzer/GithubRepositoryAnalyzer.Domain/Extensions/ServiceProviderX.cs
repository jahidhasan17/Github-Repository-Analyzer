using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GithubRepositoryAnalyzer.Domain.Extensions;

public static class ServiceProviderX
{
    public static void AddDatabaseSettings(this IServiceCollection services)
    {
        // services.AddSingleton<IConfigureOptions<SqlSettings>, ConfigureSqlSettings>();
    }
}