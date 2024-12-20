using GithubRepositoryAnalyzer.EventMessaging.Contracts;
using GithubRepositoryAnalyzer.EventMessaging.Contracts.GithubRepositoryAnalyzer;

namespace GithubRepositoryAnalyzer.EventMessaging.Worker.RepositorySearchService;

public interface IRepositorySearchService
{
    Task<List<SearchRepositoryItem>> FindRepositories(SearchRepositories searchConfig, string githubAuthToken);
}