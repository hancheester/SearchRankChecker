using SearchRankChecker.Application.Common;

namespace SearchRankChecker.Application.Search.Dto;

public class SearchHistoryDto : BaseEntity
{
    public string SearchTerm { get; set; }
    public string TargetUrl { get; set; }
    public string SearchEngine { get; set; }
    public DateTime SearchDate { get; set; }
    public string Rankings { get; set; }
    public ICollection<SearchResultEntryDto> SearchResultEntries { get; set; }
    public bool ShowEntries { get; set; }
}