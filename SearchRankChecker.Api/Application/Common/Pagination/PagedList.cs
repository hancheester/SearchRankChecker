using System.Collections.Generic;

namespace SearchRankChecker.Api.Application.Common.Pagination;

public interface IPagedList<T>
{
    int PageIndex { get; set; }
    int PageSize { get; set; }
    int TotalPages { get; set; }
    int TotalCount { get; set; }
}

public class PagedList<T> : IPagedList<T>
{
    public IList<T> Items { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => (PageIndex + 1) < TotalPages;
}