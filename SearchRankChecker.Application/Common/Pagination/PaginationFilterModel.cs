namespace SearchRankChecker.Application.Common.Pagination;

public class PaginationFilterModel : IColumnFilterModel, ISortingOrder
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public IDictionary<string, SortingOrder> Order { get; set; }
    public IDictionary<string, string> ColumnSearch { get; set; }
}

public interface ISortingOrder
{
    public IDictionary<string, SortingOrder> Order { get; set; }
}

public interface IColumnFilterModel
{
    public IDictionary<string, string> ColumnSearch { get; set; }
}

public enum SortingOrder
{
    Asc,
    Desc
}
