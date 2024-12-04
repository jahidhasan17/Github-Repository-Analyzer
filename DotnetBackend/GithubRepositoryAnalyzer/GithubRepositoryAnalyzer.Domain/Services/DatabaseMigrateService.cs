using GithubRepositoryAnalyzer.Domain.Context;
using Microsoft.EntityFrameworkCore;

namespace GithubRepositoryAnalyzer.Domain.Services;

public class DatabaseMigrateService : IDatabaseMigrateService
{
    private readonly ConfigContext configContext;

    public DatabaseMigrateService(ConfigContext configContext)
    {
        this.configContext = configContext;
    }

    /// <inheritdoc />
    public Task MigrateAsync()
    {
        return configContext.Database.MigrateAsync();
    }
}