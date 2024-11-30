using System.Collections.Generic;

namespace SearchRankChecker.Api.Application.Search.Dto;

public class SearchResult
{
    public ICollection<SearchResultEntryDto> Results { get; set; }
}
