using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SearchRankChecker.Api.Entities;
using SearchRankChecker.Api.Infrastructure.Persistence;

namespace SearchRankChecker.Api.Infrastructure.Mapping;

public class SearchHistoryMap : EntityTypeConfiguration<SearchHistory>
{
    public override void Configure(EntityTypeBuilder<SearchHistory> builder)
    {
        builder.ToTable(nameof(SearchHistory));
        builder.HasKey(x => x.Id);
    }
}
