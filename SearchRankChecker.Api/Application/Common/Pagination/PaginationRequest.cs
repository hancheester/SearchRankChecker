using System.Collections.Generic;

namespace SearchRankChecker.Api.Application.Common.Pagination;

public abstract class PaginationRequest
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    //public string Keyword { get; set; }
    public IDictionary<string, SortingOrder> Order { get; set; }
    public IDictionary<string, string> ColumnSearch { get; set; }
    //public ICollection<string> KeywordColumns { get; set; }
}

public enum SortingOrder
{
    Asc,
    Desc
}
