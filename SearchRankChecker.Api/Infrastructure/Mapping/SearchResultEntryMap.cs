using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SearchRankChecker.Api.Entities;
using SearchRankChecker.Api.Infrastructure.Persistence;

namespace SearchRankChecker.Api.Infrastructure.Mapping;

public class SearchResultEntryMap : EntityTypeConfiguration<SearchResultEntry>
{
    public override void Configure(EntityTypeBuilder<SearchResultEntry> builder)
    {
        builder.ToTable(nameof(SearchResultEntry));
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.SearchHistory)
               .WithMany(x => x.SearchResultEntries)
               .HasForeignKey(x => x.SearchHistoryId)
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("FK_SearchResultEntry_SearchHistory");
    }
}
