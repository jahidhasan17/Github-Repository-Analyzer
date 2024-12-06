using AngleSharp;
using GithubRepositoryAnalyzer.EventMessaging.Contracts;
using GithubRepositoryAnalyzer.EventMessaging.Contracts.GithubRepositoryAnalyzer;
using GithubRepositoryAnalyzer.Kernel.Cache;
using MassTransit;

namespace GithubRepositoryAnalyzer.EventMessaging.Worker;

public class SearchRepositoriesConsumer(
    IHttpClientFactory httpClientFactory,
    ICacheStorage<SearchRepositoryResult> cache) : IConsumer<SearchRepositories>
{
    public async Task Consume(ConsumeContext<SearchRepositories> context)
    {
        var searchModel = context.Message;
        var correlationId = context.CorrelationId.ToString();
        var userToSearch = context.Headers.Get<int>("UserToSearch") ?? 0;
        
        if(correlationId == null)
        {
            throw new InvalidOperationException("CorrelationId is null");
        }
        
        var searchResult = await FindRepositories(searchModel, null);
        
        cache.AddOrUpdate(correlationId, new SearchRepositoryResult
        {
            ReposResult = searchResult,
            FollowingUserSearchCompleted = searchModel.GithubHandles.Count,
            TotalSearchRequired = userToSearch
        });
    }
    
    public async Task<List<SearchRepositoryItem>> FindRepositories(SearchRepositories searchConfig, string githubAuthToken)
    {
        var searchResult = new List<SearchRepositoryItem>();
        var client = httpClientFactory.CreateClient();

        var tasks = searchConfig.GithubHandles.Select(user =>
            FetchRepositoriesForUser(client, searchConfig.SearchText, searchConfig.SearchLanguage, user, githubAuthToken));

        var batchResults = await Task.WhenAll(tasks);

        foreach (var repos in batchResults)
        {
            searchResult.AddRange(repos);
        }

        return searchResult;
    }

    private async Task<List<SearchRepositoryItem>> FetchRepositoriesForUser(HttpClient client, string searchText, string searchLanguage, string user, string githubAuthToken)
    {
        var url = $"https://github.com/{user}?tab=repositories&q={searchText}&type=&language={searchLanguage}&sort=";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        if (!string.IsNullOrEmpty(githubAuthToken))
        {
            request.Headers.Add("Authorization", $"token {githubAuthToken}");
        }

        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            return new List<SearchRepositoryItem>();
        }

        var content = await response.Content.ReadAsStringAsync();
        
        return await ParseRepositoryInfo(content, user);
    }

    private async Task<List<SearchRepositoryItem>> ParseRepositoryInfo(string html, string user)
    {
        var context = BrowsingContext.New(Configuration.Default);
        var document = await context.OpenAsync(req => req.Content(html));
        var repositoryList = new List<SearchRepositoryItem>();

        foreach (var element in document.QuerySelectorAll("#user-repositories-list > ul > li"))
        {
            var repo = new SearchRepositoryItem
            {
                UserName = user,
                Name = element.QuerySelector("div > div > h3 > a")?.TextContent.Trim(),
                ForkName = element.QuerySelector("div > div > span > a")?.TextContent.Trim(),
                Description = element.QuerySelector("div > div:nth-child(2) > p")?.TextContent.Trim(),
                Language = element.QuerySelector("div > div.f6.color-fg-muted.mt-2 > span > span:nth-child(2)")?.TextContent.Trim(),
                Star = element.QuerySelector("div > div.f6.color-fg-muted.mt-2 > a:nth-child(2)")?.TextContent.Trim(),
                Fork = element.QuerySelector("div > div.f6.color-fg-muted.mt-2 > a:nth-child(3)")?.TextContent.Trim(),
                ModifiedDate = element.QuerySelector("div > div.f6.color-fg-muted.mt-2 > relative-time")?.TextContent.Trim()
            };

            repositoryList.Add(repo);
        }

        return repositoryList;
    }
}