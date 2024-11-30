using System.Collections.Generic;

namespace SearchRankChecker.Api.Application.Common.Pagination;

public class PaginationFilterModel : ColumnFilterModel
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public IDictionary<string, SortingOrder> Order { get; set; }
}

public abstract class ColumnFilterModel
{
    public IDictionary<string, string> ColumnSearch { get; set; }
}
