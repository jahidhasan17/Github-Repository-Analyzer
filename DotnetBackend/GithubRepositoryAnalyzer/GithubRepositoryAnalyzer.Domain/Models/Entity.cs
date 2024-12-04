namespace GithubRepositoryAnalyzer.Domain.Models;

public class Entity
{
    public string Id { get; set; }

    public virtual void RecalculateIds()
    {
        if (string.IsNullOrEmpty(Id))
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}