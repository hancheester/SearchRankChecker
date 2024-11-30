using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SearchRankChecker.Api.Application.Common;
using SearchRankChecker.Api.Infrastructure.Persistence;
using System;

namespace SearchRankChecker.Api.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
        services.AddTransient(typeof(IUnitOfWork), typeof(UnitOfWork));
        services.AddTransient<IEntityTypeProvider, EntityTypeProvider>();
        services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();

        return services;
    }
}
