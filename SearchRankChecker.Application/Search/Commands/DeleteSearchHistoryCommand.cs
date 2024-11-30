using MediatR;
using SearchRankChecker.Application.Common.ApiClient;
using SearchRankChecker.Application.Common.Result;
using SearchRankChecker.Application.Common.Routes;

namespace SearchRankChecker.Application.Search.Commands;

public class DeleteSearchHistoryCommand : IRequest<IResult>
{
    public int Id { get; set; }
}

public class DeleteSearchHistoryCommandHandler : IRequestHandler<DeleteSearchHistoryCommand, IResult>
{
    private readonly IApiClientFactory _apiClientFactory;

    public DeleteSearchHistoryCommandHandler(IApiClientFactory apiClientFactory)
    {
        _apiClientFactory = apiClientFactory;
    }

    public async Task<IResult> Handle(DeleteSearchHistoryCommand request, CancellationToken cancellationToken)
    {
        var uri = SearchApiEndpoints.DeleteHistory(request.Id);
        var apiClient = _apiClientFactory.CreateClient(ApiClientName.SearchApi);

        return await apiClient.DeleteAsync(uri, cancellationToken);
    }
}