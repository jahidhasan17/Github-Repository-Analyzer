using GithubRepositoryAnalyzer.EventMessaging.Contracts;
using GithubRepositoryAnalyzer.EventMessaging.Contracts.GithubRepositoryAnalyzer;
using GithubRepositoryAnalyzer.EventMessaging.Worker.RepositorySearchService;
using GithubRepositoryAnalyzer.Kernel.Cache;
using MassTransit;

namespace GithubRepositoryAnalyzer.EventMessaging.Worker;

public class SearchRepositoriesConsumer(
    IRepositorySearchService repositorySearchService,
    ICacheStorage<SearchRepositoryResult> cache,
    ILogger<SearchRepositoriesConsumer> logger) : IConsumer<SearchRepositories>
{
    public async Task Consume(ConsumeContext<SearchRepositories> context)
    {
        var retryAttempt = context.GetRetryAttempt();
        var searchModel = context.Message;
        var correlationId = context.CorrelationId.ToString();
        var userToSearch = context.Headers.Get<int>("UserToSearch") ?? 0;
        var moreUserToSearch = context.Headers.Get<bool>("MoreUserToSearch") ?? false;

        try
        {
            if (correlationId == null)
            {
                throw new InvalidOperationException("CorrelationId is null");
            }

            var searchResult = await repositorySearchService.FindRepositories(searchModel, null);

            var cacheKey = $"{correlationId}:{Guid.NewGuid().ToString()}:MyValue";

            cache.AddOrUpdate(cacheKey, new SearchRepositoryResult
            {
                ReposResult = searchResult,
                FollowingUserSearchCompleted = searchModel.GithubHandles.Count,
                TotalSearchRequired = userToSearch,
                MoreUserToSearch = moreUserToSearch,
                SearchState = SearchState.Completed,
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error Occured from Consumer {nameof(SearchRepositoriesConsumer)}");

            if (retryAttempt == 2)
            {
                var cacheKey = $"{correlationId}:{Guid.NewGuid().ToString()}:MyValue";

                cache.AddOrUpdate(cacheKey, new SearchRepositoryResult
                {
                    ReposResult = [],
                    FollowingUserSearchCompleted = searchModel.GithubHandles.Count,
                    TotalSearchRequired = userToSearch,
                    MoreUserToSearch = moreUserToSearch,
                    SearchState = SearchState.Error,
                });
            }
            
            throw;
        }
    }
}