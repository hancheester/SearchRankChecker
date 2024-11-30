using SearchRankChecker.Application.Common.ApiClient;

namespace SearchRankChecker.Infrastructure;

public class ApiClientFactory : IApiClientFactory
{
    private readonly IHttpClientFactory _factory;

    public ApiClientFactory(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public IApiClient CreateClient(ApiClientName name)
    {
        return new ApiClient(_factory.CreateClient(name.ToString()));
    }
}
