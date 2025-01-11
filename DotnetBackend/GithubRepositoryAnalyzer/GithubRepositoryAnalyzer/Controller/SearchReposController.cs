using GithubRepositoryAnalyzer.Dto;
using GithubRepositoryAnalyzer.EventMessaging.Contracts;
using GithubRepositoryAnalyzer.EventMessaging.Contracts.GithubRepositoryAnalyzer;
using GithubRepositoryAnalyzer.Kernel.Cache;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api")]
public class SearchReposController(
    ISendEndpointProvider sendEndpointProvider,
    ICacheStorage<SearchRepositoryResult> cache,
    ILogger<SearchReposController> logger)
    : ControllerBase
{
    
    [HttpGet("get/repositories")]
    public async Task<IActionResult> GetRepositories(string searchId)
    {
        var staticData = new
        {
            reposResult = new[]
            {
                new
                {
                    name = "Algorithms",
                    userName = "imovishek",
                    forkName = "PetarV-/Algorithms",
                    description = "Several algorithms and data structures implemented in C++ by me (credited to others where necessary).",
                    language = "C++",
                    star = "1",
                    fork = (string?)null,
                    modifiedDate = "May 17, 2019"
                },
                new
                {
                    name = "LearnAlgoDesktop",
                    userName = "imovishek",
                    forkName = "haque-lubna/LearnAlgoDesktop",
                    description = "JAVA project",
                    language = "Java",
                    star = (string?)null,
                    fork = (string?)null,
                    modifiedDate = "Aug 16, 2018"
                },
                new
                {
                    name = "Data-Structures-and-Algorithms-Notebook-Bangla",
                    userName = "mhRumi",
                    forkName = "KhanShaheb34/Data-Structures-and-Algorithms-Notebook",
                    description = "Some DS and Algorithms implementation in various languages and Notebook in Bangla",
                    language = "C++",
                    star = "5",
                    fork = (string?)null,
                    modifiedDate = "Mar 26, 2023"
                }
            },
            followingUserSearchCompleted = 30,
            totalSearchRequired = 0,
            moreUserToSearch = true,
            searchState = "Completed"
        };
        
        return Ok(new
        {
            Data = staticData
        });
    }
    
    /*[HttpGet("get/repositories")]
    public async Task<IActionResult> GetRepositories(string searchId)
    {
        var searchResult = cache.GetValuesByPattern($"{searchId}*");
        
        if (searchResult == null)
        {
            return Ok(new
            {
                Message = "Search Id was not Found.",
                Data = new SearchRepositoryResult
                {
                    SearchState = SearchState.Processing,
                }
            });
        }

        var searchRepositoryResults = searchResult.ToList();
        var firstSearchResult = searchRepositoryResults.MaxBy(s => s.TotalSearchRequired);
        
        var searchCompleted = searchRepositoryResults.Sum(x => x.FollowingUserSearchCompleted);

        if (searchCompleted >= firstSearchResult?.TotalSearchRequired)
        {
            return Ok(new
            {
                Data = new SearchRepositoryResult
                {
                    ReposResult = searchRepositoryResults.SelectMany(d => d.ReposResult).ToList(),
                    FollowingUserSearchCompleted = searchRepositoryResults.Sum(d => d.FollowingUserSearchCompleted),
                    MoreUserToSearch = firstSearchResult.MoreUserToSearch,
                    SearchState = SearchState.Completed,
                }
            });
        }
        
        return Ok(new
        {
            Data = new SearchRepositoryResult
            {
                SearchState = SearchState.Processing,
            }
        });
    }*/
    
    [HttpPost("search/repositories")]
    public async Task<IActionResult> SearchRepositories([FromBody] SearchRepositoryQuery query)
    {
        var correlationId = Guid.NewGuid();
        
        return Ok(new
        {
            Message = "Repositories retrieved successfully.",
            Data = new
            {
                SearchId = correlationId,
            }
        });
        
        logger.LogCritical("Current Searching Correlation Id - " + correlationId);
        
        var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("queue:SearchRepositoryOnUserNetwork"));
        await sendEndpoint.Send(new SearchRepositoryOnUserNetwork
        {
            GithubHandle = query.GithubHandle,
            SearchText = query.QueryText,
            SearchLanguage = query.QueryLanguage,
            SearchStartIndexFromFollowingUsers = query.QuerySearchStartIndex
        }, context =>
        {
            context.CorrelationId = correlationId;
        });
        
        var cacheKey = $"{correlationId}:{Guid.NewGuid().ToString()}:MyValue";
        
        cache.AddOrUpdate(cacheKey, new SearchRepositoryResult
        {
            ReposResult = new List<SearchRepositoryItem>(),
            FollowingUserSearchCompleted = 0,
            TotalSearchRequired = 0,
        });

        return Ok(new
        {
            Message = "Repositories retrieved successfully.",
            Data = new
            {
                SearchId = correlationId,
            }
        });
    }
}
