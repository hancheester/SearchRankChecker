using MediatR;
using SearchRankChecker.Application.Common.ApiClient;
using SearchRankChecker.Application.Common.Result;
using SearchRankChecker.Application.Common.Routes;
using SearchRankChecker.Application.Search.Dto;

namespace SearchRankChecker.Application.Search.Queries;

public class SearchQuery : IRequest<IResult<SearchResult>>
{
    public SearchDto Model { get; set; }
}

public class SearchQueryHandler : IRequestHandler<SearchQuery, IResult<SearchResult>>
{
    private readonly IApiClient _apiClient;

    public SearchQueryHandler(IApiClientFactory apiClientFactory)
    {
        _apiClient = apiClientFactory.CreateClient(ApiClientName.SearchApi);
    }

    public async Task<IResult<SearchResult>> Handle(SearchQuery request, CancellationToken cancellationToken)
    {
        var uri = SearchApiEndpoints.Search;
        return await _apiClient.PutAsync<SearchDto, SearchResult>(uri, request.Model, cancellationToken);
    }
}
