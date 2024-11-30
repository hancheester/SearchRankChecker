using System.Net;

namespace SearchRankChecker.Application.Common.Result;

public interface IResult
{
    List<string> Messages { get; set; }
    bool Succeeded { get; set; }
    HttpStatusCode StatusCode { get; set; }
}

public interface IResult<out T> : IResult
{
    T Data { get; }
}
