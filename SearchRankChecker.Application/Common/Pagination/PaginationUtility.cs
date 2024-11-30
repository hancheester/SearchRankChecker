using System.Collections.Specialized;
using System.Web;

namespace SearchRankChecker.Application.Common.Pagination;

public sealed class PaginationUtility
{
    public static NameValueCollection GenerateQueryString(
        string queryInfo,
        int pageIndex,
        int pageSize,
        IDictionary<string, SortingOrder> order = null,
        IDictionary<string, string> columnSearch = null)
    {
        var query = HttpUtility.ParseQueryString(queryInfo);

        if (pageIndex > 0) query["pageIndex"] = pageIndex.ToString();
        if (pageSize > 0) query["pageSize"] = pageSize.ToString();

        if (order != null && order.Count > 0)
        {
            var i = 0;
            foreach (var item in order)
            {
                query[$"order[{i}].key"] = item.Key;
                query[$"order[{i}].value"] = item.Value.ToString();
                i++;
            }
        }

        if (columnSearch != null && columnSearch.Count > 0)
        {
            var i = 0;
            foreach (var item in columnSearch)
            {
                query[$"columnSearch[{i}].key"] = item.Key;
                query[$"columnSearch[{i}].value"] = item.Value;
                i++;
            }
        }

        return query;
    }
}