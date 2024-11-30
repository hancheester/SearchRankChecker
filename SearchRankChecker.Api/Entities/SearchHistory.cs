using System;
using System.Collections.Generic;

namespace SearchRankChecker.Api.Entities;

public class SearchHistory : BaseEntity
{
    public string SearchTerm { get; set; }
    public string TargetUrl { get; set; }
    public string SearchEngine { get; set; }
    public DateTime SearchDate { get; set; }
    public virtual ICollection<SearchResultEntry> SearchResultEntries { get; set; }
}
