using SearchRankChecker.Api.Application.Common.Mappings;
using SearchRankChecker.Api.Entities;

namespace SearchRankChecker.Api.Application.Search.Dto;

public class SearchResultEntryDto : IMapFrom<SearchResultEntry>
{
    public string Url { get; set; }
    public int Rank { get; set; }
}