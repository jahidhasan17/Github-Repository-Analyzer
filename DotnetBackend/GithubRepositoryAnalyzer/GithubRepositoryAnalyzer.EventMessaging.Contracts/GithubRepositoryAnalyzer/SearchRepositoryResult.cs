namespace GithubRepositoryAnalyzer.EventMessaging.Contracts.GithubRepositoryAnalyzer;

public class SearchRepositoryResult
{
    public IEnumerable<SearchRepositoryItem> ReposResult { get; set; }
    
    public int FollowingUserSearchCompleted { get; set; }
    
    public int TotalSearchRequired { get; set; }
    
    public bool MoreUserToSearch { get; set; }

    public SearchState SearchState { get; set; }
}