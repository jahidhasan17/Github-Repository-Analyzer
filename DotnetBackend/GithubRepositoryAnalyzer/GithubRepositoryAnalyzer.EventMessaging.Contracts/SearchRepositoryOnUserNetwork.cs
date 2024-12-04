namespace GithubRepositoryAnalyzer.EventMessaging.Contracts;

public class SearchRepositoryOnUserNetwork
{
    public string GithubHandle { get; set; }

    public string SearchText { get; set; }

    public string SearchLanguage { get; set; }

    public int SearchStartIndexFromFollowingUsers { get; set; }
}