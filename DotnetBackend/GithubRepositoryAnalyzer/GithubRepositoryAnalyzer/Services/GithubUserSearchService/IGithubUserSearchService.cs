using GithubRepositoryAnalyzer.Dto;

namespace GithubRepositoryAnalyzer.Services.GithubUserSearchService;

public interface IGithubUserSearchService
{
    Task<List<GithubUser>> GetUserGithubInfo(List<string> githubHandles, string githubAuthToken);
    
    Task<List<string>> GetAllFollowingUsers(List<GithubUser> users, string githubAuthToken);
}