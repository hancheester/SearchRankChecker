using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;
using System.Security.Cryptography;

namespace SearchRankChecker.Api.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly IEntityTypeProvider _entityTypeProvider;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IEntityTypeProvider entityTypeProvider)
        : base(options)
    {
        _entityTypeProvider = entityTypeProvider;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        foreach (var type in _entityTypeProvider.GetEntityTypeConfigurations(Assembly.GetExecutingAssembly().GetTypes(),
                                                                             typeof(EntityTypeConfiguration<>)))
        {
            var configuration = (IMappingConfiguration)Activator.CreateInstance(type);
            configuration.ApplyConfiguration(builder);
        }
    }
}
