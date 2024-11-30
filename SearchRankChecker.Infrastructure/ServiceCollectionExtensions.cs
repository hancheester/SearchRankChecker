using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SearchRankChecker.Application.Common.ApiClient;
using SearchRankChecker.Application.Common.Extensions;

namespace SearchRankChecker.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var searchApiUrl = configuration.GetValue<string>("SearchApiUrl") ?? throw new ArgumentNullException("SearchApiUrl is not configured");
        searchApiUrl = searchApiUrl.EnsureEndsWith("/");

        services.AddScoped<IApiClientFactory, ApiClientFactory>();

        services.AddHttpClient(nameof(ApiClientName.SearchApi), c =>
        {
            c.BaseAddress = new Uri(searchApiUrl);
        });

        return services;
    }
}