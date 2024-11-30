using MediatR;
using Microsoft.Extensions.Logging;
using SearchRankChecker.Api.Application.Common.Result;
using SearchRankChecker.Api.Application.Search.Commands;
using SearchRankChecker.Api.Application.Search.Dto;
using SearchRankChecker.Api.Application.Search.Strategy;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SearchRankChecker.Api.Application.Search.Queries;

public class SearchQuery : IRequest<IResult<SearchResult>>
{
    public SearchDto Model { get; set; }
}

public class SearchQueryHandler : IRequestHandler<SearchQuery, IResult<SearchResult>>
{
    private const int MAX_RESULTS = 100;
    private readonly HttpClient _httpClient;
    private readonly IPublisher _mediator;
    private readonly ILogger<SearchQueryHandler> _logger;

    public SearchQueryHandler(HttpClient httpClient, IPublisher mediator, ILogger<SearchQueryHandler> logger)
    {
        _httpClient = httpClient;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<IResult<SearchResult>> Handle(SearchQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var searchEngineType = (SearchEngineType)Enum.Parse(typeof(SearchEngineType), request.Model.SearchEngine, true);
            var searchEngine = new SearchEngine(_httpClient);

            var results = await searchEngine.SearchPageRanking(searchEngineType, request.Model.SearchTerm, request.Model.Url);
            var entries = results.Select(result => new SearchResultEntryDto
            {
                Rank = result.Rank,
                Url = result.Url,
            }).ToList();

            await _mediator.Publish(new SearchResultCreatedNotification
            {
                SearchHistory = request.Model,
                Results = entries
            }, cancellationToken);

            return await Result<SearchResult>.SuccessAsync(new SearchResult { Results = entries });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while searching.");
            return await Result<SearchResult>.FailAsync($"Sorry, an error has occurred. Please try again later. {ex.Message}");
        }
    }
}