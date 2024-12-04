using AngleSharp;
using GithubRepositoryAnalyzer.Dto;

namespace GithubRepositoryAnalyzer.Services.RepositorySearchService;

public class RepositorySearchService(IHttpClientFactory httpClientFactory) : IRepositorySearchService
{
    public async Task<SearchRepositoryResult> FindRepositories(SearchRepositoryConfiguration config, List<string> users, string githubAuthToken)
    {
        var searchResult = new SearchRepositoryResult { ReposResult = new List<SearchRepositoryItem>() };
        var client = httpClientFactory.CreateClient();

        for (int batchStartIndex = config.SearchStartIndex; batchStartIndex < users.Count; batchStartIndex += config.PageSearchPerApiCall)
        {
            var userBatch = users
                .Skip(batchStartIndex)
                .Take(config.PageSearchPerApiCall)
                .ToList();

            var tasks = userBatch.Select(user => FetchRepositoriesForUser(client, config, user, githubAuthToken));

            var batchResults = await Task.WhenAll(tasks);

            foreach (var repos in batchResults)
            {
                searchResult.ReposResult.AddRange(repos);
            }

            if (searchResult.ReposResult.Count < config.MinDesireRepository)
            {
                continue;
            }
            
            searchResult.HasMoreUserToSearch = true;
            searchResult.UserSearchDoneCount = batchStartIndex + userBatch.Count;
            
            return searchResult;
        }
        
        searchResult.HasMoreUserToSearch = false;
        searchResult.UserSearchDoneCount = users.Count;

        return searchResult;
    }

    private async Task<List<SearchRepositoryItem>> FetchRepositoriesForUser(HttpClient client, SearchRepositoryConfiguration config, string user, string githubAuthToken)
    {
        var url = $"https://github.com/{user}?tab=repositories&q={config.SearchText}&type=&language={config.SearchLanguage}&sort=";

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
