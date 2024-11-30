using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SearchRankChecker.Api.Infrastructure;

namespace SearchRankChecker.Api;

internal static class ApplicationBuilderExtensions
{
    internal static IApplicationBuilder Initialize(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var seeder = serviceScope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
        seeder.Initialize();

        return app;
    }
}
