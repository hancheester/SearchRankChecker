using SearchRankChecker.Application.Common.Extensions;
using SearchRankChecker.Application.Common.Result;
using System.Net.Http.Json;

namespace SearchRankChecker.Application.Common.ApiClient;

public interface IApiClient
{
    Task<IResult<T>> GetAsync<T>(string uri, CancellationToken cancellationToken);
    Task<IResult<TResult>> PutAsync<TContent, TResult>(string uri, TContent content, CancellationToken cancellationToken);
    Task<IResult> DeleteAsync(string uri, CancellationToken cancellationToken);
}

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IResult> DeleteAsync(string uri, CancellationToken cancellationToken)
    {
        var response = await _httpClient.DeleteAsync(uri, cancellationToken);
        return await response.ToResult();
    }

    public async Task<IResult<T>> GetAsync<T>(string uri, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(uri, cancellationToken);
        return await response.ToResult<T>();
    }

    public async Task<IResult<TResult>> PutAsync<TContent, TResult>(string uri, TContent content, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PutAsJsonAsync(uri, content, cancellationToken);
        return await response.ToResult<TResult>();
    }
}
