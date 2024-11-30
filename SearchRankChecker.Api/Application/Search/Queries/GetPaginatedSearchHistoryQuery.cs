using MediatR;
using Microsoft.EntityFrameworkCore;
using SearchRankChecker.Api.Application.Common;
using SearchRankChecker.Api.Application.Common.Pagination;
using SearchRankChecker.Api.Application.Common.Result;
using SearchRankChecker.Api.Application.Search.Dto;
using SearchRankChecker.Api.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SearchRankChecker.Api.Application.Common.Extensions;
using AutoMapper;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace SearchRankChecker.Api.Application.Search.Queries;

public class GetPaginatedSearchHistoryQuery : PaginationRequest, IRequest<IResult<IPagedList<SearchHistoryDto>>>
{
}

public class GetPaginatedSearchHistoryQueryHandler : IRequestHandler<GetPaginatedSearchHistoryQuery, IResult<IPagedList<SearchHistoryDto>>>
{
    private const int DEFAULT_PAGE_SIZE = 10;
    private const int MAX_PAGE_SIZE = 100;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetPaginatedSearchHistoryQueryHandler> _logger;

    public GetPaginatedSearchHistoryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetPaginatedSearchHistoryQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IResult<IPagedList<SearchHistoryDto>>> Handle(GetPaginatedSearchHistoryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _unitOfWork.Repository<SearchHistory>().Entities
                        .Include(x => x.SearchResultEntries)
                        .OrderByDescending(x => x.SearchDate)
                        .AsQueryable();

            if (request.ColumnSearch != null && request.ColumnSearch.Count > 0)
            {
                foreach (var column in request.ColumnSearch.Keys)
                {
                    (var canSkip, query) = CustomColumnFilter(query, column, request.ColumnSearch[column]);
                    if (!canSkip)
                    {
                        query = query.Contains(column, request.ColumnSearch[column]);
                    }
                }
            }

            if (request.Order != null && request.Order.Count > 0)
            {
                foreach (var key in request.Order.Keys)
                {
                    switch (request.Order[key])
                    {
                        case SortingOrder.Asc:
                            query = query.OrderBy(key);
                            break;
                        case SortingOrder.Desc:
                            query = query.OrderByDescending(key);
                            break;
                        default:
                            break;
                    }
                }
            }

            var pageSize = request.PageSize > MAX_PAGE_SIZE ? DEFAULT_PAGE_SIZE : request.PageSize <= 0 ? DEFAULT_PAGE_SIZE : request.PageSize;
            var pageIndex = request.PageIndex <= 0 ? 0 : request.PageIndex;

            var result = await query.ToPaginatedListAsync(
                        pageIndex,
                        pageSize,
                        _mapper.Map<List<SearchHistoryDto>>);

           return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting search history.");
            return await Result<IPagedList<SearchHistoryDto>>.FailAsync("Sorry, an error has occurred. Please try again later.");
        }
    }

    private (bool, IQueryable<SearchHistory>) CustomColumnFilter(IQueryable<SearchHistory> query, string column, string columnKeyword)
    {
        var canSkip = false;

        if (string.IsNullOrEmpty(columnKeyword))
            return (true, query);

        if (column == nameof(SearchHistory.SearchTerm))
        {
            query = query.Where(x => x.SearchTerm.Contains(columnKeyword));
            canSkip = true;
        }

        if (column == nameof(SearchHistory.TargetUrl))
        {
            query = query.Where(x => x.TargetUrl.Contains(columnKeyword));
            canSkip = true;
        }

        if (column == nameof(SearchHistory.SearchEngine))
        {
            query = query.Where(x => x.SearchEngine.Contains(columnKeyword));
            canSkip = true;
        }

        if (column == "StartDate")
        {
            var startDate = DateTime.Parse(columnKeyword);
            query = query.Where(x => x.SearchDate >= startDate);
            canSkip = true;
        }

        if (column == "EndDate")
        {
            var endDate = DateTime.Parse(columnKeyword);
            endDate = endDate.AddDays(1).Subtract(new TimeSpan(0, 0, 1));
            query = query.Where(x => x.SearchDate <= endDate);
            canSkip = true;
        }

        return (canSkip, query);
    }
}
