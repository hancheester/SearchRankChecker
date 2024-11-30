using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SearchRankChecker.Api.Entities;

namespace SearchRankChecker.Api.Infrastructure.Persistence;

public class EntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>, IMappingConfiguration
    where TEntity : BaseEntity
{
    public void ApplyConfiguration(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(this);
    }

    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
    }
}


