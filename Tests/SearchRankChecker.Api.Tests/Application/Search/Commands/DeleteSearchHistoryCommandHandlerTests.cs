using Microsoft.Extensions.Logging;
using Moq;
using SearchRankChecker.Api.Application.Common;
using SearchRankChecker.Api.Application.Search.Commands;
using SearchRankChecker.Api.Entities;

namespace SearchRankChecker.Api.Tests.Application.Search.Commands;

public class DeleteSearchHistoryCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<DeleteSearchHistoryCommandHandler>> _loggerMock;
    private readonly DeleteSearchHistoryCommandHandler _handler;

    public DeleteSearchHistoryCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<DeleteSearchHistoryCommandHandler>>();
        _handler = new DeleteSearchHistoryCommandHandler(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenSearchHistoryExists()
    {
        // Arrange
        var command = new DeleteSearchHistoryCommand { Id = 1 };
        var searchHistory = new SearchHistory();

        _unitOfWorkMock.Setup(u => u.Repository<SearchHistory>().GetById(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(searchHistory);

        _unitOfWorkMock.Setup(u => u.Repository<SearchHistory>().Delete(It.IsAny<SearchHistory>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.Commit(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(It.IsAny<int>()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        _unitOfWorkMock.Verify(u => u.Repository<SearchHistory>().Delete(searchHistory), Times.Once);
        _unitOfWorkMock.Verify(u => u.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSearchHistoryDoesNotExist()
    {
        // Arrange
        var command = new DeleteSearchHistoryCommand { Id = 1 };

        _unitOfWorkMock.Setup(u => u.Repository<SearchHistory>().GetById(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SearchHistory)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("Search history not found.", result.Messages[0]);
        _unitOfWorkMock.Verify(u => u.Repository<SearchHistory>().Delete(It.IsAny<SearchHistory>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenExceptionOccurs()
    {
        // Arrange
        var command = new DeleteSearchHistoryCommand { Id = 1 };

        _unitOfWorkMock.Setup(u => u.Repository<SearchHistory>().GetById(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("Sorry, an error has occurred. Please try again later.", result.Messages[0]);
    }
}
