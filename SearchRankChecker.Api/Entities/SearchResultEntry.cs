namespace SearchRankChecker.Api.Entities;

public class SearchResultEntry : BaseEntity
{
    public int SearchHistoryId { get; set; }
    public string Url { get; set; }
    public int Rank { get; set; }
    public virtual SearchHistory SearchHistory { get; set; }
}