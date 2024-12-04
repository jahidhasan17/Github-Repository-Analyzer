namespace GithubRepositoryAnalyzer.Domain.Models;

public interface IEntity
{
    string Id { get; set; }

    void RecalculateIds();
}
