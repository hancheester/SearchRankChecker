using FluentValidation;

namespace SearchRankChecker.Application.Common.Validation;

public abstract class BaseValidator<TDto> : AbstractValidator<TDto>
{
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<TDto>.CreateWithOptions((TDto)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();

        return result.Errors.Select(e => e.ErrorMessage);
    };
}
