namespace SearchRankChecker.Application.Common.ApiClient;

public interface IApiClientFactory
{
    IApiClient CreateClient(ApiClientName name);
}
