using System.Collections.Generic;

namespace SearchRankChecker.Api.Application.Common.Result;

public interface IResult
{
    List<string> Messages { get; set; }
    bool Succeeded { get; set; }
}

public interface IResult<out T> : IResult
{
    T Data { get; }
}
