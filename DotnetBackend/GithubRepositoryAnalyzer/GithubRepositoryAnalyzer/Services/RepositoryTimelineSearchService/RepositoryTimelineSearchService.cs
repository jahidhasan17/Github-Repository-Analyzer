using System.Net.Http.Headers;
using AngleSharp;
using AngleSharp.Dom;
using GithubRepositoryAnalyzer.Dto;

namespace GithubRepositoryAnalyzer.Services.RepositoryTimelineSearchService;

public class RepositoryTimelineSearchService(IHttpClientFactory httpClientFactory) : IRepositoryTimelineSearchService
{
    public List<TimelineReposResult> ModifiedTimelineRepositories(
        List<RepositoryItem> reposList, string firstUserName, string secondUserName)
    {
        var firstUserRepos = reposList.Where(item => item.UserName == firstUserName).ToList();
        var secondUserRepos = reposList.Where(item => item.UserName == secondUserName).ToList();

        var reposResult = new List<TimelineReposResult>();

        foreach (var repo in firstUserRepos)
        {
            reposResult.Add(new TimelineReposResult
            {
                UserName = repo.UserName,
                Name = repo.Name,
                Description = repo.Description,
                ForkName = repo.ForkName,
                Languages = repo.Languages,
                Star = repo.Star,
                Fork = repo.Fork,
                CreatedDate = repo.CreatedDate?.Replace(",", ""),
                ModifiedDate = repo.ModifiedDate?.Replace(",", ""),
                IsFirstUsersRepos = true,
                IsSecondUsersRepos = false
            });
        }

        foreach (var repo in secondUserRepos)
        {
            reposResult.Add(new TimelineReposResult
            {
                UserName = repo.UserName,
                Name = repo.Name,
                Description = repo.Description,
                ForkName = repo.ForkName,
                Languages = repo.Languages,
                Star = repo.Star,
                Fork = repo.Fork,
                CreatedDate = repo.CreatedDate?.Replace(",", ""),
                ModifiedDate = repo.ModifiedDate?.Replace(",", ""),
                IsFirstUsersRepos = false,
                IsSecondUsersRepos = true
            });
        }

        reposResult = reposResult
            .OrderBy(repo => 
                DateTime.TryParse(repo.CreatedDate, out var createdDate) ? createdDate : DateTime.MaxValue)
            .ToList();

        return reposResult;
    }
    
    public async Task<List<RepositoryItem>> FindRepositoryOfTwoUsersAsync(List<SearchRepositoryTimelineConfiguration> users, string githubAuthToken)
    {
        var client = httpClientFactory.CreateClient();

        var tasks = users.Select(user => FetchUserRepositories(client, user, githubAuthToken));

        var repos = await Task.WhenAll(tasks);
        var userRepos = repos.SelectMany(x => x).ToList();
        
        // await PopulateRepositoryDetails(client, userRepos, githubAuthToken);
        // await PopulateRepositoryFileDetails(client, userRepos, githubAuthToken);

        return userRepos;
    }

    private async Task<List<RepositoryItem>> FetchUserRepositories(HttpClient client, SearchRepositoryTimelineConfiguration user, string githubAuthToken)
    {
        var url = $"https://github.com/{user.UserName}?page={Math.Ceiling((double)(user.StartFrom + 1) / 30)}&tab=repositories";
        
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        if (!string.IsNullOrEmpty(githubAuthToken))
        {
            request.Headers.Add("Authorization", $"token {githubAuthToken}");
        }

        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            return new List<RepositoryItem>();
        }

        var content = await response.Content.ReadAsStringAsync();

        return await ParseRepositoryInfo(content, user);
    }

    private async Task PopulateRepositoryDetails(HttpClient client, List<RepositoryItem> repos, string githubAuthToken)
    {
        var tasks = repos.Select(repo =>
        {
            var url = $"https://github.com/{repo.UserName}/{repo.Name}";
            return FetchHtmlAsync(repo, client, url, githubAuthToken);
        });

        var results = await Task.WhenAll(tasks);

        foreach (var (repo, response) in results)
        {
            if (response != null)
            {
                repo.Languages = GetRepositoryLanguages(response, 4);
                repo.MainBranchName = GetMainBranchName(response);
            }
        }
    }

    private async Task PopulateRepositoryFileDetails(HttpClient client, List<RepositoryItem> repos, string githubAuthToken)
    {
        var tasks = repos.Select(repo =>
        {
            var url = $"https://github.com/{repo.UserName}/{repo.Name}/file-list/{repo.MainBranchName}";
            return FetchHtmlAsync(repo, client, url, githubAuthToken);
        });

        var results = await Task.WhenAll(tasks);

        foreach (var (repo, response) in results)
        {
            if (response != null)
            {
                repo.CreatedDate = GetRepositoryCreatedDate(response);
            }
        }
    }
    
    private async Task<(T Context, IDocument Response)> FetchHtmlAsync<T>(T context, HttpClient client, string url, string githubAuthToken)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            if (!string.IsNullOrWhiteSpace(githubAuthToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("token", githubAuthToken);
            }

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var html = await response.Content.ReadAsStringAsync();
                var config = Configuration.Default;
                var contextFactory = BrowsingContext.New(config);
                var document = await contextFactory.OpenAsync(req => req.Content(html));
                return (context, document);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching URL {url}: {ex.Message}");
        }

        return (context, null);
    }
    
    private async Task<List<RepositoryItem>> ParseRepositoryInfo(string html, SearchRepositoryTimelineConfiguration user)
    {
        var context = BrowsingContext.New(Configuration.Default);
        var document = await context.OpenAsync(req => req.Content(html));
        var repositoryList = new List<RepositoryItem>();

        foreach (var element in document.QuerySelectorAll("#user-repositories-list > ul > li"))
        {
            var repo = new RepositoryItem
            {
                UserName = user.UserName,
                Name = element.QuerySelector("div > div > h3 > a")?.TextContent.Trim(),
                ForkName = element.QuerySelector("div > div > span > a")?.TextContent.Trim(),
                Description = element.QuerySelector("div > div:nth-child(2) > p")?.TextContent.Trim(),
                Languages = new List<RepositoryLanguage>
                {
                    new RepositoryLanguage
                    {
                        Name = element.QuerySelector("div > div.f6.color-fg-muted.mt-2 > span > span:nth-child(2)")?.TextContent.Trim(),
                        Percentage = null,
                    }
                },
                Star = element.QuerySelector("div > div.f6.color-fg-muted.mt-2 > a:nth-child(2)")?.TextContent.Trim(),
                Fork = element.QuerySelector("div > div.f6.color-fg-muted.mt-2 > a:nth-child(3)")?.TextContent.Trim(),
                ModifiedDate = element.QuerySelector("div > div.f6.color-fg-muted.mt-2 > relative-time")?.TextContent.Trim()
            };
            
            //element.QuerySelector("div > div.f6.color-fg-muted.mt-2 > span > span:nth-child(2)")

            repositoryList.Add(repo);
        }
        
        repositoryList.AddRange(GetFilteredRepositories(repositoryList, user.StartFrom, user.TotalRequiredRepos));

        return repositoryList;
    }

    private List<RepositoryItem> GetFilteredRepositories(List<RepositoryItem> repos, int startFrom, int totalRequired)
    {
        startFrom %= 30;
        return repos.Skip(startFrom).Take(totalRequired).ToList();
    }

    private List<RepositoryLanguage> GetRepositoryLanguages(IDocument document, int maxLanguages)
    {
        var languageElements = document.QuerySelectorAll("div > div.BorderGrid-row > div.BorderGrid-cell > ul.list-style-none > li > a > span"); // Adjust selector
        var languages = new List<RepositoryLanguage>();

        for (int i = 0; i < languageElements.Length - 1 && i < (maxLanguages + 1) / 2; i++)
        {
            languages.Add(new RepositoryLanguage
            {
                Name = languageElements[i].TextContent.Trim(),
                Percentage = languageElements[i + 1].TextContent.Trim()
            });
        }

        return languages;
    }

    private string GetMainBranchName(IDocument document)
    {
        var branchElement = document.QuerySelector("#branch-select-menu > summary > span.css-truncate-target"); // Adjust selector
        
        return branchElement?.TextContent.Trim();
    }

    private string GetRepositoryCreatedDate(IDocument document)
    {
        var dateElements = document.QuerySelectorAll("div.Details-content--hidden-not-important.js-navigation-container.js-active-navigation-container.d-md-block > div > div.color-fg-muted.text-right > time-ago"); // Adjust selector
        
        return dateElements.FirstOrDefault()?.TextContent.Trim();
    }
}
