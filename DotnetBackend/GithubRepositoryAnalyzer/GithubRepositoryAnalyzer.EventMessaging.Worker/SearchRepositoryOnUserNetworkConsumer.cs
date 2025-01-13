using GithubRepositoryAnalyzer.EventMessaging.Contracts;
using GithubRepositoryAnalyzer.EventMessaging.Contracts.GithubRepositoryAnalyzer;
using GithubRepositoryAnalyzer.EventMessaging.Worker.GithubUserSearchService;
using GithubRepositoryAnalyzer.Kernel.Cache;
using MassTransit;

namespace GithubRepositoryAnalyzer.EventMessaging.Worker;

public class SearchRepositoryOnUserNetworkConsumer(
    ICacheStorage<GithubUser> cache,
    ICacheStorage<List<string>> followingUserCache,
    IGithubUserSearchService githubUserSearchService,
    ICacheStorage<SearchRepositoryResult> repositoryCache,
    ILogger<SearchRepositoryOnUserNetworkConsumer> logger)
    : IConsumer<SearchRepositoryOnUserNetwork>
{
    public async Task Consume(ConsumeContext<SearchRepositoryOnUserNetwork> context)
    {
        var searchModel = context.Message;
   
        var cacheKey = $"SearchRepositories:GithubUser:{searchModel.GithubHandle}";
        var retryAttempt = context.GetRetryAttempt();
        var contextCorrelationId = context.CorrelationId;

        try
        {
            if (!cache.TryGetValue(cacheKey, out GithubUser userInfo))
            {
                var githubUsers = await githubUserSearchService.GetUserGithubInfo(new List<string> {searchModel.GithubHandle}, null);

                foreach (var githubUser in githubUsers)
                {
                    var newCacheKey = $"SearchRepositories:GithubUser:{githubUser.UserId}";
                    cache.AddOrUpdate(newCacheKey, githubUser);
                }
                
                userInfo = githubUsers.FirstOrDefault();
            }
            
            
            
            var cacheKeyForFollowingUser = $"SearchRepositories:FollowingUser:{searchModel.GithubHandle}";
            
            if (!followingUserCache.TryGetValue(cacheKeyForFollowingUser, out List<string> userFollowing))
            {
                userFollowing = await githubUserSearchService.GetAllFollowingUsers(userInfo, null);
                followingUserCache.AddOrUpdate(cacheKeyForFollowingUser, userFollowing);
            }

            var nextUserToSearch = userFollowing.Skip(searchModel.SearchStartIndexFromFollowingUsers).ToList();
            
            var chunkedFollowingUsers = nextUserToSearch
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / 10)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList()
                .Take(3);


            foreach (var followingUsers in chunkedFollowingUsers)
            {
                var searchRepositories = new SearchRepositories
                {
                    GithubHandles = followingUsers,
                    SearchText = searchModel.SearchText,
                    SearchLanguage = searchModel.SearchLanguage
                };

                var moreUserToSearch =
                    (searchModel.SearchStartIndexFromFollowingUsers + chunkedFollowingUsers.Sum(x => x.Count)) <
                    userFollowing.Count;

                // await context.Send(searchRepositories);
                var sendEndpoint = await context.GetSendEndpoint(new Uri("queue:SearchRepositories"));
                await sendEndpoint.Send(searchRepositories, context =>
                {
                    context.CorrelationId = contextCorrelationId;
                    context.Headers.Set("UserToSearch", chunkedFollowingUsers.Sum(x => x.Count));
                    context.Headers.Set("MoreUserToSearch", moreUserToSearch);
                });
            }
        }
        catch (Exception ex)
        {
            if (retryAttempt == 2)
            {
                repositoryCache.AddOrUpdate($"{contextCorrelationId}:{Guid.NewGuid().ToString()}:MyValue", new SearchRepositoryResult
                {
                    ReposResult = [],
                    FollowingUserSearchCompleted = 0,
                    TotalSearchRequired = 0,
                    MoreUserToSearch = false,
                    SearchState = SearchState.Error,
                });
            }
            
            logger.LogError(ex, $"Error Occured from Consumer {nameof(SearchRepositoryOnUserNetworkConsumer)}");
            
            throw;
        }
    }
}