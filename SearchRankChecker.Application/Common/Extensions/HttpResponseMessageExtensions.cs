using SearchRankChecker.Application.Common.Result;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SearchRankChecker.Application.Common.Extensions;

public static class HttpResponseMessageExtensions
{
    public static async Task EnsureSuccessStatusCodeAsync(this HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var content = await response.Content.ReadAsStringAsync();

        if (response.Content != null)
            response.Content.Dispose();

        throw new SimpleHttpResponseException(response.StatusCode, content);
    }

    public static async Task<IResult> ToResult(this HttpResponseMessage response)
    {
        string responseAsString;
        bool succeeded = false;

        try
        {
            await response.EnsureSuccessStatusCodeAsync();
            responseAsString = await response.Content.ReadAsStringAsync();
            succeeded = true;
        }
        catch (SimpleHttpResponseException se)
        {
            responseAsString = se.Message;
        }

        var result = new Result.Result();

        if (!string.IsNullOrEmpty(responseAsString))
        {
            result = JsonSerializer.Deserialize<Result.Result>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve,
            });
        }

        result.Succeeded = succeeded;
        result.StatusCode = response.StatusCode;

        if (!result.Succeeded && result.Messages.Count == 0)
            result.Messages.Add(responseAsString);

        return result;
    }

    public static async Task<IResult<T>> ToResult<T>(this HttpResponseMessage response)
    {
        string responseAsString;
        bool succeeded = false;

        try
        {
            await response.EnsureSuccessStatusCodeAsync();
            responseAsString = await response.Content.ReadAsStringAsync();
            succeeded = true;
        }
        catch (SimpleHttpResponseException se)
        {
            responseAsString = se.Message;
        }

        var result = new Result<T>();

        if (!string.IsNullOrEmpty(responseAsString))
        {
            result = JsonSerializer.Deserialize<Result<T>>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve,
            });
        }

        result.Succeeded = succeeded;
        result.StatusCode = response.StatusCode;

        if (!result.Succeeded && result.Messages.Count == 0)
            result.Messages.Add(responseAsString);

        return result;
    }
}

public class SimpleHttpResponseException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public SimpleHttpResponseException(HttpStatusCode statusCode, string content) : base(content)
    {
        StatusCode = statusCode;
    }
}
