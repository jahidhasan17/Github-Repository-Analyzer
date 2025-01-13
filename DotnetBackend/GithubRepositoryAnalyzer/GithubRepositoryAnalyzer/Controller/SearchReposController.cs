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
    public IActionResult GetRepositories(string searchId)
    {
        var searchResult = cache.GetValuesByPattern($"{searchId}*");
        
        if (!cache.AnyKeysWithPattern($"{searchId}*"))
        {
            return BadRequest();
        }

        var searchRepositoryResults = searchResult.Where(s => s.SearchState == SearchState.Completed
                                                              || s.SearchState == SearchState.Error).ToList();
        
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
    }
    
    [HttpPost("search/repositories")]
    public async Task<IActionResult> SearchRepositories([FromBody] SearchRepositoryQuery query)
    {
        var correlationId = Guid.NewGuid();
        
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
            SearchState = SearchState.Processing,
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
