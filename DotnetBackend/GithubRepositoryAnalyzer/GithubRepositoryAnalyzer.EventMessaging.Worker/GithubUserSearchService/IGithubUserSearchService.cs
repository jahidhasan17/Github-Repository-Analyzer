using GithubRepositoryAnalyzer.EventMessaging.Contracts.GithubRepositoryAnalyzer;

namespace GithubRepositoryAnalyzer.EventMessaging.Worker.GithubUserSearchService;

public interface IGithubUserSearchService
{
    Task<List<GithubUser>> GetUserGithubInfo(List<string> githubHandles, string githubAuthToken);
    
    Task<List<string>> GetAllFollowingUsers(GithubUser user, string githubAuthToken);
}