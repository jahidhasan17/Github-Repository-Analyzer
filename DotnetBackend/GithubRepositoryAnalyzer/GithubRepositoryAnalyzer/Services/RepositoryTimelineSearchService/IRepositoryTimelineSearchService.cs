using GithubRepositoryAnalyzer.Dto;

namespace GithubRepositoryAnalyzer.Services.RepositoryTimelineSearchService;

public interface IRepositoryTimelineSearchService
{
    Task<List<RepositoryItem>> FindRepositoryOfTwoUsersAsync(
        List<SearchRepositoryTimelineConfiguration> users, string githubAuthToken);

    List<TimelineReposResult> ModifiedTimelineRepositories(
        List<RepositoryItem> reposList, string firstUserName, string secondUserName);
}