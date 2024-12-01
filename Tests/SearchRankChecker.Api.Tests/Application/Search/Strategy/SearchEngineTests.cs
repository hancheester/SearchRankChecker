using Moq;
using SearchRankChecker.Api.Application.Search.Strategy;
using System.Reflection;

namespace SearchRankChecker.Api.Tests.Application.Search.Strategy;

public class SearchEngineTests
{
    private readonly Mock<ISearchEngine> _mockGoogleSearchEngine;
    private readonly Mock<ISearchEngine> _mockBingSearchEngine;
    private readonly HttpClient _httpClient;

    public SearchEngineTests()
    {
        _mockGoogleSearchEngine = new Mock<ISearchEngine>();
        _mockBingSearchEngine = new Mock<ISearchEngine>();
        _httpClient = new HttpClient();
    }

    [Fact]
    public async Task SearchPageRanking_ShouldCallGoogleSearchEngine_WhenSearchEngineTypeIsGoogle()
    {
        // Arrange
        var searchTerm = "test query";
        var targetUrl = "http://infotrack.co.uk";
        var expectedResults = new List<(int, string)> { (1, targetUrl) };

        _mockGoogleSearchEngine
            .Setup(x => x.SearchPageRanking(searchTerm, targetUrl))
            .ReturnsAsync(expectedResults);

        var searchEngine = new SearchEngine(_httpClient);
        searchEngine.GetType().GetField("_searchEngines", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(searchEngine, new Dictionary<SearchEngineType, ISearchEngine>
            {
                { SearchEngineType.Google, _mockGoogleSearchEngine.Object },
                { SearchEngineType.Bing, _mockBingSearchEngine.Object }
            });

        // Act
        var results = await searchEngine.SearchPageRanking(SearchEngineType.Google, searchTerm, targetUrl);

        // Assert
        Assert.Equal(expectedResults, results);
        _mockGoogleSearchEngine.Verify(x => x.SearchPageRanking(searchTerm, targetUrl), Times.Once);
        _mockBingSearchEngine.Verify(x => x.SearchPageRanking(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}
