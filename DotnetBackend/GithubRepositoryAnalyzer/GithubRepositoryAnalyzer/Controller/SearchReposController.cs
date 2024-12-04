using GithubRepositoryAnalyzer.Dto;
using GithubRepositoryAnalyzer.EventMessaging.Contracts;
using GithubRepositoryAnalyzer.Services.GithubUserSearchService;
using GithubRepositoryAnalyzer.Services.RepositorySearchService;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api")]
public class SearchReposController(
    IGithubUserSearchService githubUserSearchService,
    IRepositorySearchService repositorySearchService,
    ISendEndpointProvider sendEndpointProvider)
    : ControllerBase
{
    [HttpPost("search/repositories")]
    public async Task<IActionResult> SearchRepositories([FromBody] SearchRepositoryQuery query)
    {
        string githubAccessToken = Request.Headers["GithubAccessToken"];
        bool isLoggedIn = false; // !string.IsNullOrEmpty(githubAccessToken);

        /*if (isLoggedIn)
        {
            githubAccessToken = EncryptDecrypt.Decrypt(githubAccessToken);
        }*/

        // Validate request
        /*var validationResult = SearchRepositoryValidator.Validate(query);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Message);
        }*/

        /*var userInfo = await githubUserSearchService.GetUserGithubInfo([query.GithubHandle], githubAccessToken);
        var followingUsers = await githubUserSearchService.GetAllFollowingUsers(userInfo, githubAccessToken);

        var searchConfig = new SearchRepositoryConfiguration
        {
            SearchText = query.QueryText,
            SearchLanguage = query.QueryLanguage,
            IsLoggedIn = isLoggedIn,
            PageSearchPerApiCall = isLoggedIn ? 50 : 20,
            TotalPageSearchPerApiCall = isLoggedIn ? 200 : 60,
            WaitBetweenApiCall = 2000,
            MinDesireRepository = isLoggedIn ? 10 : 4,
            TotalTimeoutForApiCall = isLoggedIn ? 10000 : 8000,
            SearchStartIndex = query.QuerySearchStartIndex
        };
        
        var searchResult = await repositorySearchService.FindRepositories(searchConfig, followingUsers, githubAccessToken);*/
        
        var correlationId = Guid.NewGuid();
        
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
