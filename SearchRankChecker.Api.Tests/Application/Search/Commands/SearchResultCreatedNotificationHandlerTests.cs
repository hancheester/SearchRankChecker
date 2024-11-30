using Moq;
using SearchRankChecker.Api.Application.Common;
using SearchRankChecker.Api.Application.Search.Commands;
using SearchRankChecker.Api.Application.Search.Dto;
using SearchRankChecker.Api.Entities;

namespace SearchRankChecker.Api.Tests.Application.Search.Commands;

public class SearchResultCreatedNotificationHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly SearchResultCreatedNotificationHandler _handler;

    public SearchResultCreatedNotificationHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new SearchResultCreatedNotificationHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldAddSearchHistoryAndCommit_WhenNotificationIsValid()
    {
        // Arrange
        var notification = new SearchResultCreatedNotification
        {
            SearchHistory = new SearchDto
            {
                SearchTerm = "test term",
                SearchEngine = "Google",
                Url = "http://example.com"
            },
            Results = new List<SearchResultEntryDto>
            {
                new SearchResultEntryDto { Rank = 1, Url = "http://example1.com" },
                new SearchResultEntryDto { Rank = 2, Url = "http://example2.com" }
            }
        };

        _unitOfWorkMock.Setup(u => u.Repository<SearchHistory>().Add(It.IsAny<SearchHistory>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SearchHistory
            {
                SearchTerm = notification.SearchHistory.SearchTerm,
                SearchEngine = notification.SearchHistory.SearchEngine,
                TargetUrl = notification.SearchHistory.Url,
            });

        _unitOfWorkMock.Setup(u => u.Commit(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(It.IsAny<int>()));

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(u => u.Repository<SearchHistory>().Add(It.Is<SearchHistory>(sh =>
            sh.SearchTerm == notification.SearchHistory.SearchTerm &&
            sh.SearchEngine == notification.SearchHistory.SearchEngine &&
            sh.TargetUrl == notification.SearchHistory.Url &&
            sh.SearchResultEntries.Count == notification.Results.Count
        ), It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(u => u.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowArgumentNullException_WhenNotificationIsNull()
    {
        // Arrange
        SearchResultCreatedNotification notification = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(notification, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowArgumentNullException_WhenSearchHistoryIsNull()
    {
        // Arrange
        var notification = new SearchResultCreatedNotification
        {
            SearchHistory = null,
            Results = new List<SearchResultEntryDto>()
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(notification, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowArgumentNullException_WhenResultsAreNull()
    {
        // Arrange
        var notification = new SearchResultCreatedNotification
        {
            SearchHistory = new SearchDto
            {
                SearchTerm = "test term",
                SearchEngine = "Google",
                Url = "http://example.com"
            },
            Results = null
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(notification, CancellationToken.None));
    }
}
