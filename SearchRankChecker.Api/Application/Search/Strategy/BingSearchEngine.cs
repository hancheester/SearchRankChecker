using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace SearchRankChecker.Api.Application.Search.Strategy;

public class BingSearchEngine : ISearchEngine
{
    private readonly HttpClient _httpClient;
    private readonly int _maxResults;

    public BingSearchEngine(HttpClient httpClient, int maxResults)
    {
        _httpClient = httpClient;
        _maxResults = maxResults;
    }

    public async Task<List<(int, string)>> SearchPageRanking(string query, string targetUrl)
    {
        if (string.IsNullOrWhiteSpace(query)) throw new ArgumentNullException(nameof(query));
        if (string.IsNullOrWhiteSpace(targetUrl)) throw new ArgumentNullException(nameof(targetUrl));

        if (!_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
        {
            _httpClient.DefaultRequestHeaders.Add(
                "User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36");
        }

        var regex = new Regex(@"<a\s+[^>]*class\s*=\s*[""']tilk[""'][^>]*\bhref\s*=\s*[""']([^""']+)[""']", RegexOptions.Compiled);

        var baseUri = "https://www.bing.com/search";
        var queryStrings = new[] { 1, 23, 73 }
            .Select(start => $"{baseUri}?q={HttpUtility.UrlEncode(query)}&count={_maxResults}&first={start}")
            .ToList();

        var fetchTasks = queryStrings.Select(uri => _httpClient.GetStringAsync(uri));
        var responses = await Task.WhenAll(fetchTasks);

        var entries = new List<(int, string)>();
        int rank = 1;

        foreach (var response in responses)
        {
            if (rank > _maxResults) break;

            var matches = regex.Matches(response);
            foreach (Match match in matches)
            {
                if (rank > _maxResults) break;
                
                var url = WebUtility.HtmlDecode(match.Groups[1].Value);
                
                if (!entries.Any(e => e.Item2 == url))
                {
                    entries.Add((rank, url));
                }
                rank++;
            }
        }

        var matchingEntries = entries.Where(e => e.Item2.Contains(targetUrl)).ToList();

        return matchingEntries;
    }
}
