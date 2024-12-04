using GithubRepositoryAnalyzer.Dto;

namespace GithubRepositoryAnalyzer.Services.RepositorySearchService;

public interface IRepositorySearchService
{
    Task<SearchRepositoryResult> FindRepositories(SearchRepositoryConfiguration config, List<string> users,
        string githubAuthToken);
}