using SearchRankChecker.Api.Application.Common.Mappings;
using SearchRankChecker.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SearchRankChecker.Api.Application.Search.Dto;

public class SearchHistoryDto : IMapFrom<SearchHistory>
{
    public int Id { get; set; }
    public string SearchTerm { get; set; }
    public string TargetUrl { get; set; }
    public string SearchEngine { get; set; }
    public DateTime SearchDate { get; set; }
    public string Rankings
    {
        get
        {
            if (SearchResultEntries == null || SearchResultEntries.Count == 0)
                return "0";
            return string.Join(", ", SearchResultEntries.OrderBy(x => x.Rank).Select(x => x.Rank).ToList());
        }
    }
    public ICollection<SearchResultEntryDto> SearchResultEntries { get; set; }
}