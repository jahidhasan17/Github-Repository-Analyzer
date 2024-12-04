namespace GithubRepositoryAnalyzer.Dto;

public class SearchRepositoryTimelineConfiguration
{
    public string UserName { get; set; }
    
    public int RepositoryCount { get; set; }
    
    public int StartFrom { get; set; }
    
    public int TotalRequiredRepos { get; set; }
}