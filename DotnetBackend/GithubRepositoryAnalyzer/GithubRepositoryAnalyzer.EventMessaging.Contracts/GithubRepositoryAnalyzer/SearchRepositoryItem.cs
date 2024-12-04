namespace GithubRepositoryAnalyzer.EventMessaging.Contracts.GithubRepositoryAnalyzer;

public class SearchRepositoryItem
{
    public string Name { get; set; }

    public string UserName { get; set; }

    public string? ForkName { get; set; }

    public string? Description { get; set; }

    public string? Language { get; set; }

    public string? Star { get; set; }

    public string? Fork { get; set; }

    public string? ModifiedDate { get; set; }
}