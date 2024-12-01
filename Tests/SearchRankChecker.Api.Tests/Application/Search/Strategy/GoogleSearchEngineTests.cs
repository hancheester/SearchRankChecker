using Moq.Protected;
using Moq;
using SearchRankChecker.Api.Application.Search.Strategy;
using System.Net;

namespace SearchRankChecker.Api.Tests.Application.Search.Strategy;

public class GoogleSearchEngineTests
{
    [Fact]
    public async Task SearchPageRanking_ShouldThrowArgumentNullException_WhenQueryIsNull()
    {
        // Arrange
        var httpClient = new HttpClient();
        var engine = new GoogleSearchEngine(httpClient, 10);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => engine.SearchPageRanking(null, "http://infotrack.co.uk"));
    }

    [Fact]
    public async Task SearchPageRanking_ShouldThrowArgumentNullException_WhenTargetUrlIsNull()
    {
        // Arrange
        var httpClient = new HttpClient();
        var engine = new GoogleSearchEngine(httpClient, 10);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => engine.SearchPageRanking("land registry search", null));
    }

    [Fact]
    public async Task SearchPageRanking_ShouldReturnEmptyList_WhenNoMatchesFound()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("<html><body>No matches here</body></html>")
            });

        var httpClient = new HttpClient(mockHandler.Object);
        var engine = new GoogleSearchEngine(httpClient, 10);

        // Act
        var results = await engine.SearchPageRanking("test query", "http://infotrack.co.uk");

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public async Task SearchPageRanking_ShouldReturnMatchingResults_WhenMatchesAreFound()
    {
        // Arrange
        var mockResponse = @"<div class=""egMi0 kCrYT""><a href=""/url?q=https://www.gov.uk/get-information-about-property-and-land/search-the-register&amp;sa=U&amp;ved=2ahUKEwjmqN3trYeKAxXmTqQEHYIWJpwQFnoECGYQAg&amp;usg=AOvVaw1sSdVUi3QhbYVlJZQCCyyN"" data-ved=""2ahUKEwjmqN3trYeKAxXmTqQEHYIWJpwQFnoECGYQAg""></a></div>";
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(mockResponse)
            });

        var httpClient = new HttpClient(mockHandler.Object);
        var engine = new GoogleSearchEngine(httpClient, 10);

        // Act
        var result = await engine.SearchPageRanking("land registry", "www.gov.uk");

        // Assert
        Assert.NotEmpty(result);
        Assert.Single(result);
        Assert.Equal("https://www.gov.uk/get-information-about-property-and-land/search-the-register", result[0].Item2);
    }
}
