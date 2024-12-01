using Fluxor;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SearchRankChecker.Application.Common.Pagination;
using SearchRankChecker.Application.Search.Commands;
using SearchRankChecker.Application.Search.Dto;
using SearchRankChecker.Application.Search.Queries;
using SearchRankChecker.Client.Store.Search.Actions;

namespace SearchRankChecker.Client.Pages;

public partial class History
{
    private MudTable<SearchHistoryDto> _table;
    private bool _loading = true;
    private bool _searching = false;
    private DateRange _dateRange { get; set; }
    private string _searchTerm { get; set; }
    private string _targetUrl { get; set; }
    private string _searchEngine { get; set; }

    [Inject] private IDispatcher Dispatcher { get; set; }

    private async Task<TableData<SearchHistoryDto>> ServerReloadAsync(TableState state, CancellationToken token)
    {
        _loading = true;

        var (totalItems, items) = await LoadDataAsync(state, _searchTerm, _targetUrl, _searchEngine, _dateRange);
        
        _loading = false;

        return new TableData<SearchHistoryDto> { TotalItems = totalItems, Items = items };
    }

    private async Task<(int, IEnumerable<SearchHistoryDto>)> LoadDataAsync(TableState state, string searchTerm, string targetUrl, string searchEngine, DateRange dateRange)
    {
        var query = new GetPaginateSearchHistoryQuery
        {
            PageIndex = state.Page,
            PageSize = state.PageSize,
            ColumnSearch = new Dictionary<string, string>(),
        };

        if (!string.IsNullOrEmpty(searchTerm))
            query.ColumnSearch.Add(nameof(SearchHistoryDto.SearchTerm), searchTerm);

        if (!string.IsNullOrEmpty(_targetUrl))
            query.ColumnSearch.Add(nameof(SearchHistoryDto.TargetUrl), targetUrl);

        if (!string.IsNullOrEmpty(_searchEngine))
            query.ColumnSearch.Add(nameof(SearchHistoryDto.SearchEngine), searchEngine);

        if (dateRange != null)
        {
            if (dateRange.Start != null)
                query.ColumnSearch.Add("StartDate", dateRange.Start.Value.ToString());

            if (dateRange.End != null)
                query.ColumnSearch.Add("EndDate", dateRange.End.Value.ToString());
        }

        if (!string.IsNullOrEmpty(state.SortLabel))
        {
            var sortBy = SortingOrder.Asc;
            if (state.SortDirection == SortDirection.Descending)
            {
                sortBy = SortingOrder.Desc;
            }

            query.Order = new Dictionary<string, SortingOrder>
            {
                { state.SortLabel, sortBy },
            };
        }

        var response = await _mediator.Send(query);
        if (response.Succeeded)
        {
            return (response.Data.TotalCount, response.Data.Items);
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }

        return (0, Enumerable.Empty<SearchHistoryDto>());
    }

    private async Task SearchAsync()
    {
        _searching = true;
        await _table.ReloadServerData();
        _searching = false;
    }

    private async Task DeleteAsync(int id)
    {
        var parameters = new DialogParameters
        {
            { nameof(Shared.Dialogs.CustomConfirmation.ContentText), "Are you sure you want to delete this entry?" },
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
        var dialog = _dialogService.Show<Shared.Dialogs.CustomConfirmation>("Delete", parameters, options);

        var result = await dialog.Result;

        if (!result.Canceled)
        {
            _snackBar.Add("Deleting...", Severity.Info, options => options.CloseAfterNavigation = true);

            var response = await _mediator.Send(new DeleteSearchHistoryCommand { Id = id });
            
            _snackBar.Clear();

            if (response.Succeeded)
            {
                _snackBar.Add("Entry has been deleted successfully.", Severity.Success);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }

            await _table.ReloadServerData();
        }
    }

    private async Task RepeatSearchAsync(string searchTerm, string targetUrl, string searchEngine)
    {
        var parameters = new DialogParameters
        {
            { nameof(Shared.Dialogs.CustomConfirmation.ContentText), "Are you sure you want to repeat the search?" },
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
        var dialog = _dialogService.Show<Shared.Dialogs.CustomConfirmation>("Search", parameters, options);

        var result = await dialog.Result;

        if (!result.Canceled)
        {
            Dispatcher.Dispatch(new ResetSearchAction());
            Dispatcher.Dispatch(new SetSearchDtoAction(new SearchDto
            {
                SearchTerm = searchTerm,
                Url = targetUrl,
                SearchEngine = searchEngine,
            }));
            Dispatcher.Dispatch(new AutoSearchAction());

            _navigationManager.NavigateTo("/");
        }
    }
}