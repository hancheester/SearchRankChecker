using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using SearchRankChecker.Api.Application.Common;
using SearchRankChecker.Api.Application.Common.Pagination;
using SearchRankChecker.Api.Application.Search.Dto;
using SearchRankChecker.Api.Application.Search.Queries;
using SearchRankChecker.Api.Entities;

namespace SearchRankChecker.Api.Tests.Application.Search.Queries;

public class GetPaginatedSearchHistoryQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<GetPaginatedSearchHistoryQueryHandler>> _loggerMock;
    private readonly GetPaginatedSearchHistoryQueryHandler _handler;

    public GetPaginatedSearchHistoryQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<GetPaginatedSearchHistoryQueryHandler>>();
        _handler = new GetPaginatedSearchHistoryQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsFailResult_WhenExceptionIsThrown()
    {
        // Arrange
        var query = new GetPaginatedSearchHistoryQuery { PageIndex = 1, PageSize = 10 };
        _unitOfWorkMock.Setup(u => u.Repository<SearchHistory>().Entities).Throws(new Exception("Database error"));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("Sorry, an error has occurred. Please try again later.", result.Messages.FirstOrDefault());
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(1, 5)]
    [InlineData(-1, 15)]
    public async Task Handle_UsesDefaultPageSize_WhenInvalidPageSizeIsProvided(int pageIndex, int pageSize)
    {
        // Arrange
        var query = new GetPaginatedSearchHistoryQuery { PageIndex = pageIndex, PageSize = pageSize };
        var mockData = new List<SearchHistory>().AsQueryable();

        _unitOfWorkMock.Setup(u => u.Repository<SearchHistory>().Entities).Returns(mockData);
        _mapperMock.Setup(m => m.Map<List<SearchHistoryDto>>(It.IsAny<List<SearchHistory>>())).Returns(new List<SearchHistoryDto>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task Handle_ReturnsPaginatedResults_WhenQueryIsValid()
    {
        // Arrange
        var query = new GetPaginatedSearchHistoryQuery { PageIndex = 0, PageSize = 10 };
        var mockData = new List<SearchHistory>
        {
            new SearchHistory { Id = 1, SearchTerm = "test1" },
            new SearchHistory { Id = 2, SearchTerm = "test2" }
        }.AsQueryable();

        _unitOfWorkMock.Setup(u => u.Repository<SearchHistory>().Entities).Returns(mockData);
        _mapperMock.Setup(m => m.Map<List<SearchHistoryDto>>(It.IsAny<List<SearchHistory>>()))
                   .Returns(new List<SearchHistoryDto>
                   {
                       new SearchHistoryDto { Id = 1, SearchTerm = "test1" },
                       new SearchHistoryDto { Id = 2, SearchTerm = "test2" }
                   });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Succeeded);
        Assert.Equal(2, result.Data.TotalCount);
    }
}
