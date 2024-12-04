namespace GithubRepositoryAnalyzer.EventMessaging.Contracts.GithubRepositoryAnalyzer;

public class GithubUser
{
    public string UserId { get; set; }

    public int FollowingCount { get; set; }

    public int RepositoryCount { get; set; }
}