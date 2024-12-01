using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SearchRankChecker.Api.Application.Common;
using SearchRankChecker.Api.Application.Common.Pagination;
using SearchRankChecker.Api.Application.Search.Dto;
using SearchRankChecker.Api.Entities;
using System.Net;
using System.Net.Http.Json;

namespace SearchRankChecker.Api.IntegrationTests;

public class SearchControllerTests : IClassFixture<SearchRankCheckerApiWebApplicationFactory<Program>>
{
    private readonly SearchRankCheckerApiWebApplicationFactory<Program> _factory;

    public SearchControllerTests(SearchRankCheckerApiWebApplicationFactory<Program> factory)
    {
        _factory = factory;

        SeedDataAsync().Wait();
    }

    private async Task SeedDataAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var searchEngines = new List<string>
        {
            "Google",
            "Bing",
        };

        for (var i = 0; i < 10; i++)
        {
            await unitOfWork.Repository<SearchHistory>().Add(new SearchHistory
            {
                SearchEngine = searchEngines[i % 2],
                SearchTerm = "land registry search",
                TargetUrl = "https://www.gov.uk/",
                SearchDate = DateTime.UtcNow,
            });
        }

        await unitOfWork.Commit();
    }

    private async Task SeedDataAsync(SearchHistory history)
    {
        using var scope = _factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        await unitOfWork.Repository<SearchHistory>().Add(history);
        await unitOfWork.Commit();
    }

    [Fact]
    [Trait("Action", "Delete search history")]
    public async Task DeleteSearchHistoryAsync_ShouldReturnOk_ForValidId()
    {
        // Arrange
        var targetHistory = new SearchHistory
        {
            SearchEngine = "Google",
            SearchTerm = "land registry search",
            TargetUrl = "https://www.gov.uk/",
            SearchDate = DateTime.UtcNow,
        };
        await SeedDataAsync(targetHistory);

        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync($"/api/v1/search/history/{targetHistory.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<Application.Common.Result.Result>();
        content.Should().NotBeNull();
        content!.Succeeded.Should().BeTrue();
    }

    [Fact]
    [Trait("Action", "Delete search history")]
    public async Task DeleteSearchHistoryAsync_ShouldReturnBadRequest_ForNonExistentId()
    {
        // Arrange
        var id = 999;
        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync($"/api/v1/search/history/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadFromJsonAsync<Application.Common.Result.Result>();
        content.Should().NotBeNull();
        content!.Succeeded.Should().BeFalse();

        //var responseAsString = await response.Content.ReadAsStringAsync();
        //Assert.NotEmpty(responseAsString);

        //var result = JsonSerializer.Deserialize<Application.Common.Result.Result>(responseAsString, new JsonSerializerOptions
        //{
        //    PropertyNameCaseInsensitive = true,
        //    ReferenceHandler = ReferenceHandler.Preserve,
        //});

        //result.Should().NotBeNull();
        //result!.Succeeded.Should().BeFalse();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [Trait("Action", "Delete search history")]
    public async Task DeleteSearchHistoryAsync_ShouldReturnBadRequest_ForInvalidId(int id)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync($"/api/v1/search/history/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadFromJsonAsync<Application.Common.Result.Result>();
        content.Should().NotBeNull();
        content!.Succeeded.Should().BeFalse();
    }

    [Fact]
    [Trait("Action", "Delete search history")]
    public async Task DeleteSearchHistoryAsync_ShouldRemoveRecord_FromDatabase()
    {
        // Arrange
        var targetHistory = new SearchHistory
        {
            SearchEngine = "Google",
            SearchTerm = "land registry search",
            TargetUrl = "https://www.gov.uk/",
            SearchDate = DateTime.UtcNow,
        };

        await SeedDataAsync(targetHistory);

        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync($"/api/v1/search/history/{targetHistory.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var scope = _factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var searchHistory = await unitOfWork.Repository<SearchHistory>().GetById(targetHistory.Id);
        searchHistory.Should().BeNull();


    }

    [Fact]
    [Trait("Action", "Get paginated search history")]
    public async Task GetPaginatedSearchHistoryAsync_ShouldReturnOk_WhenDataIsValid()
    {
        // Arrange
        var query = new
        {
            PageIndex = 0,
            PageSize = 10,
            Order = new { SearchDate = "desc" },
            ColumnSearch = new { SearchTerm = "land" }
        };

        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"api/v1/search/history?PageIndex={query.PageIndex}&PageSize={query.PageSize}&Order[SearchDate]={query.Order.SearchDate}&ColumnSearch[SearchTerm]={query.ColumnSearch.SearchTerm}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Application.Common.Result.Result<PagedList<SearchHistoryDto>>>();
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.Data.Items.Should().NotBeEmpty();
    }

    [Fact]
    [Trait("Action", "Get paginated search history")]
    public async Task GetPaginatedSearchHistoryAsync_ShouldReturnFilteredResults_WhenColumnSearchIsProvided()
    {
        // Arrange
        var query = new
        {
            PageIndex = 0,
            PageSize = 10,
            ColumnSearch = new { SearchEngine = "Google" }
        };

        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"api/v1/search/history?PageIndex={query.PageIndex}&PageSize={query.PageSize}&ColumnSearch[SearchEngine]={query.ColumnSearch.SearchEngine}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Application.Common.Result.Result<PagedList<SearchHistoryDto>>>();
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.Data.Items.Should().OnlyContain(x => x.SearchEngine.Contains(query.ColumnSearch.SearchEngine));
    }

    [Fact]
    [Trait("Action", "Get paginated search history")]
    public async Task GetPaginatedSearchHistoryAsync_ShouldMatchPagination_WhenValidParametersAreProvided()
    {
        // Arrange
        var query = new
        {
            PageIndex = 1,
            PageSize = 5,
        };

        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"api/v1/search/history?PageIndex={query.PageIndex}&PageSize={query.PageSize}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Application.Common.Result.Result<PagedList<SearchHistoryDto>>>();
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.Data.Items.Count.Should().BeLessThanOrEqualTo(query.PageSize);
    }

    [Theory]
    [InlineData("Google", "google map", "google.com")]
    [InlineData("Bing", "bing", "bing.com")]
    [Trait("Action", "Search")]
    public async Task PutAsync_ShouldReturnOk_WhenDataIsValid(string searchEngine, string searchTerm, string url)
    {
        // Arrange
        var request = new
        {
            SearchEngine = searchEngine,
            SearchTerm = searchTerm,
            Url = url,
        };

        var client = _factory.CreateClient();

        // Act
        var response = await client.PutAsJsonAsync("api/v1/search", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Application.Common.Result.Result<SearchResult>>();
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.Data.Results.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("Yahoo", "", "google.com")]
    [InlineData("Invalid engine", "bing", "")]
    [InlineData("", "", "")]
    [Trait("Action", "Search")]
    public async Task PutAsync_ShouldReturnBadRequest_WhenInvalidDataIsProvided(string searchEngine, string searchTerm, string url)
    {
        // Arrange
        var request = new
        {
            SearchEngine = searchEngine,
            SearchTerm = searchTerm,
            Url = url,
        };

        var client = _factory.CreateClient();

        // Act
        var response = await client.PutAsJsonAsync("api/v1/search", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await response.Content.ReadFromJsonAsync<Application.Common.Result.Result<SearchResult>>();
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.Messages.Should().NotBeEmpty();
    }

    [Fact]
    [Trait("Action", "Search")]
    public async Task PutAsync_ShouldReturnBadRequest_WhenSearchTermIsTooLong()
    {
        // Arrange
        var request = new
        {
            SearchEngine = "Google",
            SearchTerm = new string('a', 2050), // Max length is 2048
            Url = "google.com",
        };

        var client = _factory.CreateClient();

        // Act
        var response = await client.PutAsJsonAsync("api/v1/search", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await response.Content.ReadFromJsonAsync<Application.Common.Result.Result<SearchResult>>();
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.Messages.Should().NotBeEmpty();
        result.Messages.Should().Contain("SearchTerm: Search term is too long. Please shorten your input.");
    }
}