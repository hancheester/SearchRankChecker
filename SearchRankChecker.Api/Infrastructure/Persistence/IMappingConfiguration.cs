using Microsoft.EntityFrameworkCore;

namespace SearchRankChecker.Api.Infrastructure.Persistence;

public interface IMappingConfiguration
{
    void ApplyConfiguration(ModelBuilder modelBuilder);
}
