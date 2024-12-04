using GithubRepositoryAnalyzer.Dto;
using GithubRepositoryAnalyzer.Services.GithubUserSearchService;
using GithubRepositoryAnalyzer.Services.RepositoryTimelineSearchService;
using Microsoft.AspNetCore.Mvc;

namespace GithubRepositoryAnalyzer.Controller;

[ApiController]
[Route("api")]
public class RepositoryTimelineController(IGithubUserSearchService userSearchService, IRepositoryTimelineSearchService repositoryTimelineSearchService) : ControllerBase
{
    [HttpPost("github/timeline/repository")]
    public async Task<IActionResult> SearchRepositories([FromBody] List<SearchRepositoryTimeline> query)
    {
        var userInfos = await userSearchService.GetUserGithubInfo([query[0].UserName, query[1].UserName], string.Empty);

        var searchQuery = new List<SearchRepositoryTimelineConfiguration>
        {
            new()
            {
                UserName = query[0].UserName,
                RepositoryCount = userInfos[0].RepositoryCount,
                StartFrom = query[0].StartFrom,
                TotalRequiredRepos = 10,
            },
            new()
            {
                UserName = query[1].UserName,
                RepositoryCount = userInfos[1].RepositoryCount,
                StartFrom = query[1].StartFrom,
                TotalRequiredRepos = 10,
            }
        };
        
        var result = await repositoryTimelineSearchService.FindRepositoryOfTwoUsersAsync(searchQuery, string.Empty);
        
        var firstUserReposCount = result.Count(x => x.UserName == query[0].UserName);
        var secondUserReposCount = result.Count(x => x.UserName == query[1].UserName);
        
        var timelineResult = repositoryTimelineSearchService.ModifiedTimelineRepositories(result, query[0].UserName, query[1].UserName);
        
        return new JsonResult(new
        {
            message = "Fetch Successful",
            data = timelineResult,
            TotalFirstUserRepoSearchDone = searchQuery[0].StartFrom + firstUserReposCount,
            TotalSecondUserRepoSearchDone = searchQuery[1].StartFrom + secondUserReposCount,
            TotalFirstUserRepoFound = firstUserReposCount,
            TotalSecondUserRepoFound = secondUserReposCount,
            HasMoreReposForFirstUser = (searchQuery[0].StartFrom + firstUserReposCount) < userInfos[0].RepositoryCount,
            HasMoreReposForSecondUser = (searchQuery[1].StartFrom + secondUserReposCount) < userInfos[1].RepositoryCount
        })
        {
            StatusCode = StatusCodes.Status200OK
        };
    }
}