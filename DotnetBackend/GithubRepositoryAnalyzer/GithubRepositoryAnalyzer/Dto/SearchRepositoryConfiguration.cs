namespace GithubRepositoryAnalyzer.Dto;

public class SearchRepositoryConfiguration
{
    public string SearchText { get; set; }
    
    public string SearchLanguage { get; set; }
    
    public bool IsLoggedIn { get; set; }
    
    public int PageSearchPerApiCall { get; set; }
    
    public int TotalPageSearchPerApiCall { get; set; }
    
    public int WaitBetweenApiCall { get; set; }
    
    public int MinDesireRepository { get; set; }
    
    public int TotalTimeoutForApiCall { get; set; }
    
    public int SearchStartIndex { get; set; }
}