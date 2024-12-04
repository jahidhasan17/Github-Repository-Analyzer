namespace GithubRepositoryAnalyzer.Dto;

public class SearchRepositoryQuery
{
    public string GithubHandle { get; set; }

    public string QueryText { get; set; }

    public string QueryLanguage { get; set; }

    public int QuerySearchStartIndex { get; set; }
}