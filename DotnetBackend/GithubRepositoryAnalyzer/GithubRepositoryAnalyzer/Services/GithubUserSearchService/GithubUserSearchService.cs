using AngleSharp;
using GithubRepositoryAnalyzer.Dto;

namespace GithubRepositoryAnalyzer.Services.GithubUserSearchService;

public class GithubUserSearchService(IHttpClientFactory httpClientFactory) : IGithubUserSearchService
{
    public async Task<List<GithubUser>> GetUserGithubInfo(List<string> githubHandles, string githubAuthToken)
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

    public async Task<List<string>> GetAllFollowingUsers(List<GithubUser> users, string githubAuthToken)
    {
        var client = httpClientFactory.CreateClient();
        var tasks = new List<Task<(string userId, string content)>>();

        foreach (var user in users)
        {
            for (int i = 1; i <= (user.FollowingCount + 49) / 50; i++)
            {
                var url = $"https://github.com/{user.UserId}?page={i}&tab=following";
                tasks.Add(FetchFollowingPageAsync(client, url, githubAuthToken, user.UserId));
            }
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
