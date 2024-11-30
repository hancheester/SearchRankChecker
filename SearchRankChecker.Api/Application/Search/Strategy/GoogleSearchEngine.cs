using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace SearchRankChecker.Api.Application.Search.Strategy;

public class GoogleSearchEngine : ISearchEngine
{
    private readonly HttpClient _httpClient;
    private readonly int _maxResults;

    public GoogleSearchEngine(HttpClient httpClient, int maxResults)
    {
        _httpClient = httpClient;
        _maxResults = maxResults;
    }

    public async Task<List<(int, string)>> SearchPageRanking(string query, string targetUrl)
    {
        if (string.IsNullOrWhiteSpace(query)) throw new ArgumentNullException(nameof(query));
        if (string.IsNullOrWhiteSpace(targetUrl)) throw new ArgumentNullException(nameof(targetUrl));

        var uri = $"https://www.google.co.uk/search?q={HttpUtility.UrlEncode(query)}&num={_maxResults}";
        var response = await _httpClient.GetStringAsync(uri);

        var regex = @"(?<=<div class=""egMi0 kCrYT""><a href=""/url\?q=)[^""]*";
        var matches = Regex.Matches(response, regex);

        var entries = new List<(int, string)>();
        int rank = 1;
        foreach (Match match in matches)
        {
            if (match.Groups[0].Value.Contains(targetUrl))
            {
                var q = match.Groups[0].Value;
                var url = q.Substring(0, q.IndexOf("&amp;sa="));
                entries.Add((rank, WebUtility.HtmlDecode(url)));
            }
            rank++;
        }

        return entries;
    }
}
