using FluentValidation.TestHelper;
using SearchRankChecker.Api.Application.Search.Dto;

namespace SearchRankChecker.Api.Tests.Application.Search.Dto;

public class SearchDtoValidatorTests
{
    private readonly SearchDtoValidator _validator;

    public SearchDtoValidatorTests()
    {
        _validator = new SearchDtoValidator();
    }

    [Theory]
    [InlineData("", "http://example.com", "Google", "Search term is required.")]
    [InlineData("a", "", "Google", "Url is required.")]
    [InlineData("a", "http://example.com", "", "Search engine is required.")]
    public void Should_HaveValidationError_WhenInputIsInvalid(string searchTerm, string url, string searchEngine, string expectedErrorMessage)
    {
        // Arrange
        var dto = new SearchDto { SearchTerm = searchTerm, Url = url, SearchEngine = searchEngine };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        if (string.IsNullOrEmpty(searchTerm))
        {
            result.ShouldHaveValidationErrorFor(x => x.SearchTerm)
                  .WithErrorMessage(expectedErrorMessage);
        }
        if (string.IsNullOrEmpty(url))
        {
            result.ShouldHaveValidationErrorFor(x => x.Url)
                  .WithErrorMessage(expectedErrorMessage);
        }
        if (string.IsNullOrEmpty(searchEngine))
        {
            result.ShouldHaveValidationErrorFor(x => x.SearchEngine)
                  .WithErrorMessage(expectedErrorMessage);
        }
    }

    [Fact]
    public void Should_HaveValidationError_WhenSearchTermExceedsMaximumLength()
    {
        // Arrange
        var longSearchTerm = new string('a', 2049);
        var dto = new SearchDto { SearchTerm = longSearchTerm, Url = "http://example.com", SearchEngine = "Google" };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SearchTerm)
              .WithErrorMessage("Search term is too long. Please shorten your input.");
    }

    [Fact]
    public void Should_NotHaveValidationErrors_WhenDtoIsValid()
    {
        // Arrange
        var dto = new SearchDto { SearchTerm = "test term", Url = "http://example.com", SearchEngine = "Google" };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
