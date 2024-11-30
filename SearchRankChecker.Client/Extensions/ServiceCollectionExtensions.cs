using Fluxor;
using MudBlazor;
using MudBlazor.Services;

namespace SearchRankChecker.Client.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddClient(this IServiceCollection services)
    {
        services.AddMudServices(configuration =>
        {
            configuration.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
            configuration.SnackbarConfiguration.HideTransitionDuration = 100;
            configuration.SnackbarConfiguration.ShowTransitionDuration = 100;
            configuration.SnackbarConfiguration.VisibleStateDuration = 30000;
            configuration.SnackbarConfiguration.ShowCloseIcon = true;
        });

        services.AddFluxor(options =>
        {
            options.ScanAssemblies(typeof(Program).Assembly);
        });

        return services;
    }
}
