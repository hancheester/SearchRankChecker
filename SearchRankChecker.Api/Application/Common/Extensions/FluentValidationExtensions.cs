using System.Linq;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using FluentValidation.Results;

namespace SearchRankChecker.Api.Application.Common.Extensions;

public static class FluentValidationExtensions
{
    public static List<string> ToMessages(this ValidationResult result)
    {
        return result.Errors
            .GroupBy(x => x.PropertyName)
            .Select(x => $"{x.Key}: {string.Join(", ", x.Select(y => y.ErrorMessage))}")
            .ToList();
    }
}
