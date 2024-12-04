namespace GithubRepositoryAnalyzer.EventMessaging.Contracts;

public class SearchRepositories
{
    public List<string> GithubHandles { get; set; }

    public string SearchText { get; set; }

    public string SearchLanguage { get; set; }
}