using Moq;
using Moq.Protected;
using SearchRankChecker.Api.Application.Search.Strategy;
using System.Net;

namespace SearchRankChecker.Api.Tests.Application.Search.Strategy;

public class BingSearchEngineTests
{
    [Fact]
    public async Task SearchPageRanking_ShouldThrowArgumentNullException_WhenQueryIsNull()
    {
        // Arrange
        var httpClient = new HttpClient();
        var engine = new BingSearchEngine(httpClient, 10);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => engine.SearchPageRanking(null, "http://lalslsls.com"));
    }

    [Fact]
    public async Task SearchPageRanking_ShouldThrowArgumentNullException_WhenTargetUrlIsNull()
    {
        // Arrange
        var httpClient = new HttpClient();
        var engine = new BingSearchEngine(httpClient, 10);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => engine.SearchPageRanking("search term", null));
    }

    [Fact]
    public async Task SearchPageRanking_ShouldReturnEmptyList_WhenNoMatchesFound()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                var response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("<html><body>No matches here</body></html>")
                };

                // Prevent stream disposal by cloning the content
                var buffer = response.Content.ReadAsByteArrayAsync().Result;
                response.Content = new StringContent(System.Text.Encoding.UTF8.GetString(buffer));

                return response;
            }
        );

        var httpClient = new HttpClient(mockHandler.Object);
        var engine = new BingSearchEngine(httpClient, 10);

        // Act
        var result = await engine.SearchPageRanking("search term", "http://sample.com");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchPageRanking_ShouldReturnMatchingResults_WhenMatchesAreFound()
    {
        // Arrange
        var mockResponse = @"<a class=""tilk"" href=""http://sample.com"">Sample</a>";
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                var response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(mockResponse)
                };

                // Prevent stream disposal by cloning the content
                var buffer = response.Content.ReadAsByteArrayAsync().Result;
                response.Content = new StringContent(System.Text.Encoding.UTF8.GetString(buffer));

                return response;
            }
        );

        var httpClient = new HttpClient(mockHandler.Object);
        var engine = new BingSearchEngine(httpClient, 10);

        // Act
        var result = await engine.SearchPageRanking("search term", "http://sample.com");

        // Assert
        Assert.NotEmpty(result);
        Assert.Single(result);
        Assert.Equal("http://sample.com", result[0].Item2);
    }
}
