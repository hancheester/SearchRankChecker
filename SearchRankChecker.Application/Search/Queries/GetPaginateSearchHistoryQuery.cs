using MediatR;
using SearchRankChecker.Application.Common.ApiClient;
using SearchRankChecker.Application.Common.Pagination;
using SearchRankChecker.Application.Common.Result;
using SearchRankChecker.Application.Common.Routes;
using SearchRankChecker.Application.Search.Dto;

namespace SearchRankChecker.Application.Search.Queries;

public class GetPaginateSearchHistoryQuery : PaginationFilterModel, IRequest<IResult<PagedList<SearchHistoryDto>>>
{
}

public class GetPaginateSearchHistoryQueryHandler : IRequestHandler<GetPaginateSearchHistoryQuery, IResult<PagedList<SearchHistoryDto>>>
{
    private readonly IApiClientFactory _apiClientFactory;

    public GetPaginateSearchHistoryQueryHandler(IApiClientFactory apiClientFactory)
    {
        _apiClientFactory = apiClientFactory;
    }

    public async Task<IResult<PagedList<SearchHistoryDto>>> Handle(GetPaginateSearchHistoryQuery request, CancellationToken cancellationToken)
    {
        var uri = SearchApiEndpoints.History;
        var uriBuilder = new UriBuilder(uri);

        var query = PaginationUtility.GenerateQueryString(
            uriBuilder.Query,
            request.PageIndex,
            request.PageSize,
            request.Order,
            request.ColumnSearch);

        uriBuilder.Query = query.ToString();

        var apiClient = _apiClientFactory.CreateClient(ApiClientName.SearchApi);
        return await apiClient.GetAsync<PagedList<SearchHistoryDto>>(uri + uriBuilder.Query, cancellationToken);
    }
}
