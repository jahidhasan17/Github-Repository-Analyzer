using AngleSharp;
using GithubRepositoryAnalyzer.EventMessaging.Contracts;
using GithubRepositoryAnalyzer.EventMessaging.Contracts.GithubRepositoryAnalyzer;
using GithubRepositoryAnalyzer.Kernel.Cache;
using MassTransit;

namespace GithubRepositoryAnalyzer.EventMessaging.Worker;

public class SearchRepositoryOnUserNetworkConsumer(
    IHttpClientFactory httpClientFactory,
    ICacheStorage<GithubUser> cache,
    ICacheStorage<List<string>> followingUserCache)
    : IConsumer<SearchRepositoryOnUserNetwork>
{
    public async Task Consume(ConsumeContext<SearchRepositoryOnUserNetwork> context)
    {
        var searchModel = context.Message;
   
        // chceck if the key exists in the cache
        var cacheKey = $"SearchRepositories:GithubUser:{searchModel.GithubHandle}";


        if (!cache.TryGetValue(cacheKey, out GithubUser userInfo))
        {
            var githubUsers = await GetUserGithubInfo(new List<string> {searchModel.GithubHandle}, null);

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
            userFollowing = await GetAllFollowingUsers(userInfo, null);
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

            // await context.Send(searchRepositories);
            var sendEndpoint = await context.GetSendEndpoint(new Uri("queue:SearchRepositories"));
            await sendEndpoint.Send(searchRepositories, context =>
            {
                context.CorrelationId = context.CorrelationId;
                context.Headers.Set("UserToSearch", chunkedFollowingUsers.Sum(x => x.Count));
            });
        }
    }
    
    private async Task<List<GithubUser>> GetUserGithubInfo(List<string> githubHandles, string? githubAuthToken)
    {
        var client = httpClientFactory.CreateClient();

        var tasks = githubHandles.Select(async githubHandle =>
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://github.com/{githubHandle}");

            if (!string.IsNullOrEmpty(githubAuthToken))
            {
                request.Headers.Add("Authorization", $"token {githubAuthToken}");
            }

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode(); // Ensure the call was successful
            var content = await response.Content.ReadAsStringAsync();

            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(content));

            var followingCount = int.Parse(document.QuerySelector("div.mb-3 > a.Link--secondary:nth-child(2) > span")?.TextContent.Trim() ?? "0");
            var repositoryCount = int.Parse(document.QuerySelector("div.Layout-main > div > nav > a > span")?.TextContent.Trim() ?? "0");

            return new GithubUser
            {
                UserId = githubHandle,
                FollowingCount = followingCount,
                RepositoryCount = repositoryCount
            };
        });

        var githubUsers = await Task.WhenAll(tasks); // Execute all tasks concurrently
        return githubUsers.ToList();
    }
    
    private async Task<List<string>> GetAllFollowingUsers(GithubUser user, string githubAuthToken)
    {
        var client = httpClientFactory.CreateClient();
        var tasks = new List<Task<(string userId, string content)>>();

        for (int i = 1; i <= (user.FollowingCount + 49) / 50; i++)
        {
            var url = $"https://github.com/{user.UserId}?page={i}&tab=following";
            tasks.Add(FetchFollowingPageAsync(client, url, githubAuthToken, user.UserId));
        }

        var responses = await Task.WhenAll(tasks);

        var followingUsers = new List<string>();
        var context = BrowsingContext.New(Configuration.Default);
        foreach (var (userId, content) in responses)
        {
            if (!string.IsNullOrEmpty(content))
            {
                var document = await context.OpenAsync(req => req.Content(content));
                foreach (var element in document.QuerySelectorAll("a > span.Link--secondary"))
                {
                    followingUsers.Add(element.TextContent.Trim());
                }
            }
        }

        return followingUsers.Distinct().ToList(); // Deduplicate results
    }
    
    private async Task<(string userId, string content)> FetchFollowingPageAsync(HttpClient client, string url, string githubAuthToken, string userId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        if (!string.IsNullOrEmpty(githubAuthToken))
        {
            request.Headers.Add("Authorization", $"token {githubAuthToken}");
        }

        var response = await client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return (userId, content);
        }

        return (userId, string.Empty); // Return empty content on failure
    }
}