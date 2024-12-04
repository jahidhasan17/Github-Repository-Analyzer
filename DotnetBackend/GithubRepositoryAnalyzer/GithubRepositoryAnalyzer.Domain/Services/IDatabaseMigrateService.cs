namespace GithubRepositoryAnalyzer.Domain.Services;

public interface IDatabaseMigrateService
{
    /// <summary>
    /// Helper to Migrate Database.
    /// </summary>
    /// <returns>Task.</returns>
    Task MigrateAsync();
}