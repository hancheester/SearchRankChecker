using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using SearchRankChecker.Api.Application.Search.Commands;
using SearchRankChecker.Api.Application.Search.Dto;
using SearchRankChecker.Api.Application.Search.Queries;

namespace SearchRankChecker.Api.Tests.Application.Search.Queries;

public class SearchQueryHandlerTests
{
    private readonly HttpClient _httpClientMock;
    private readonly Mock<IPublisher> _mediatorMock;
    private readonly Mock<ILogger<SearchQueryHandler>> _loggerMock;
    private readonly SearchQueryHandler _handler;
    private Mock<HttpMessageHandler> _httpMessageHandlerMock;

    public SearchQueryHandlerTests()
    {
        _mediatorMock = new Mock<IPublisher>();
        _loggerMock = new Mock<ILogger<SearchQueryHandler>>();
        
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClientMock = new HttpClient(_httpMessageHandlerMock.Object);

        _handler = new SearchQueryHandler(_httpClientMock, _mediatorMock.Object, _loggerMock.Object);
    }

    [Theory]
    [InlineData("Google", "test search", "http://example.com", true)]
    [InlineData("Bing", "", "http://example.com", false)]
    [InlineData("Yahoo", "test search", "", false)]
    public async Task Handle_ReturnsExpectedResult_BasedOnInput(string searchEngine, string searchTerm, string url, bool expectedSuccess)
    {
        // Arrange
        var searchDto = new SearchDto { SearchEngine = searchEngine, SearchTerm = searchTerm, Url = url };
        var query = new SearchQuery { Model = searchDto };

        _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage { Content = new StringContent("[{'Rank': 1, 'Url': 'http://example.com'}]") });
       
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(expectedSuccess, result.Succeeded);
    }

    [Fact]
    public async Task Handle_PublishesNotification_WhenSearchResultsAreAvailable()
    {
        // Arrange
        var searchDto = new SearchDto { SearchEngine = "Google", SearchTerm = "test", Url = "http://example.com" };
        var query = new SearchQuery { Model = searchDto };

        _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage { Content = new StringContent("[{'Rank': 1, 'Url': 'http://example.com'}]") });

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(m => m.Publish(It.IsAny<SearchResultCreatedNotification>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsFailResult_WhenExceptionIsThrown()
    {
        // Arrange
        var searchDto = new SearchDto { SearchEngine = "Google", SearchTerm = "test", Url = "http://example.com" };
        var query = new SearchQuery { Model = searchDto };

        _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new Exception("Network error"));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("Sorry, an error has occurred", result.Messages.FirstOrDefault());
    }
}