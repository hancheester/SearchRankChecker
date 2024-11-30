using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SearchRankChecker.Api.Application.Search.Strategy;

public enum SearchEngineType
{
    Google,
    Bing
}

public interface ISearchEngine
{
    Task<List<(int, string)>> SearchPageRanking(string query, string targetUrl);
}

public class SearchEngine
{
    private const int MAX_RESULTS = 100;
    private readonly Dictionary<SearchEngineType, ISearchEngine> _searchEngines;

    public SearchEngine(HttpClient httpClient)
    {
        _searchEngines = new Dictionary<SearchEngineType, ISearchEngine>
        {
            { SearchEngineType.Google, new GoogleSearchEngine(httpClient, MAX_RESULTS) },
            { SearchEngineType.Bing, new BingSearchEngine(httpClient, MAX_RESULTS) }
        };
    }

    public async Task<List<(int Rank, string Url)>> SearchPageRanking(SearchEngineType type, string query, string targetUrl)
    {
        if (_searchEngines.TryGetValue(type, out var searchEngine))
        {
            return await searchEngine.SearchPageRanking(query, targetUrl);
        }
        else
        {
            throw new ArgumentException("Invalid search engine type");
        }
    }
}
