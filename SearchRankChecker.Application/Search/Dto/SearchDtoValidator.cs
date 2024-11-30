using FluentValidation;
using SearchRankChecker.Application.Common.Validation;

namespace SearchRankChecker.Application.Search.Dto;

public class SearchDtoValidator : BaseValidator<SearchDto>
{
    public SearchDtoValidator()
    {
        RuleFor(x => x.SearchTerm)
            .NotEmpty()
            .WithMessage("Search term is required.")
            .MaximumLength(2048)
            .WithMessage("Search term is too long. Please shorten your input.");

        RuleFor(x => x.Url)
            .NotEmpty()
            .WithMessage("Url is required.");

        RuleFor(x => x.SearchEngine)
            .NotEmpty()
            .WithMessage("Search engine is required.");
    }
}