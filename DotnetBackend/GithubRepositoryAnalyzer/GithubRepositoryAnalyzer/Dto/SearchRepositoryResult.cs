namespace GithubRepositoryAnalyzer.Dto;

public class SearchRepositoryResult
{
    public List<SearchRepositoryItem> ReposResult { get; set; }
    
    public int UserSearchDoneCount { get; set; }
    
    public bool HasMoreUserToSearch { get; set; }
}