using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using SearchRankChecker.Api.Application.Common.Result;
using System.Linq;
using SearchRankChecker.Api.Application.Common.Extensions;

namespace SearchRankChecker.Api.Filters;

public class ValidateDtoFilter<TValidator, TDto> : IActionFilter
    where TValidator : IValidator
    where TDto : class
{
    private readonly IValidator<TDto> _validator;

    public ValidateDtoFilter(IValidator<TDto> validator)
    {
        _validator = validator;
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        TDto dto = context.ActionArguments.Values.OfType<TDto>().FirstOrDefault();

        if (dto == null)
        {
            context.Result = new BadRequestObjectResult($"Parameter of type {nameof(TDto)} is not found.");
            return;
        }

        var validationResult = _validator.Validate(dto);

        if (!validationResult.IsValid)
            context.Result = new BadRequestObjectResult(Result.Fail(validationResult.ToMessages()));

        return;
    }
}