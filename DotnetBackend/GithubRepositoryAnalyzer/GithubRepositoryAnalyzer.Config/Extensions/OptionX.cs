using GithubRepositoryAnalyzer.Config.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GithubRepositoryAnalyzer.Config.Extensions;

public static class OptionX
{
    public static void AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOption<ConnectionSettings>(configuration);
    }
    
    private static void AddOption<T>(this IServiceCollection services, IConfiguration configuration)
        where T : class
    {
        services.Configure<T>(configuration.GetSection(typeof(T).Name));
    }
}