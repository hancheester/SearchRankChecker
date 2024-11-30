using MediatR;
using SearchRankChecker.Api.Application.Common;
using SearchRankChecker.Api.Application.Search.Dto;
using SearchRankChecker.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SearchRankChecker.Api.Application.Search.Commands;

public class SearchResultCreatedNotification : INotification
{
    public SearchDto SearchHistory { get; set; }
    public ICollection<SearchResultEntryDto> Results { get; set; }
}

public class SearchResultCreatedNotificationHandler : INotificationHandler<SearchResultCreatedNotification>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchResultCreatedNotificationHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SearchResultCreatedNotification notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification, nameof(notification));
        ArgumentNullException.ThrowIfNull(notification.SearchHistory, nameof(notification.SearchHistory));
        ArgumentNullException.ThrowIfNull(notification.Results, nameof(notification.Results));

        await _unitOfWork.Repository<SearchHistory>().Add(new SearchHistory
        {
            SearchTerm = notification.SearchHistory.SearchTerm,
            SearchEngine = notification.SearchHistory.SearchEngine,
            TargetUrl = notification.SearchHistory.Url,
            SearchDate = DateTime.UtcNow,
            SearchResultEntries = notification.Results.Select(x => new SearchResultEntry
            {
                Rank = x.Rank,
                Url = x.Url,
            }).ToList()
        });

        await _unitOfWork.Commit(cancellationToken);
    }
}
