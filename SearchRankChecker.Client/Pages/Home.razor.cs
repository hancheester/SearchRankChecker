using Fluxor;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SearchRankChecker.Application.Search.Dto;
using SearchRankChecker.Application.Search.Queries;
using SearchRankChecker.Client.Store.Search;

namespace SearchRankChecker.Client.Pages;

public partial class Home
{
    private SearchDto _model => State.Value.Model;
    private bool _autoSearch => State.Value.AutoSearch;
    private SearchResult? _searchResult;
    private bool _isBusy;
    private MudForm _form = new();

    [Inject] private IState<SearchState> State { get; set; }
    [Inject] public required SearchDtoValidator Validator { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (_autoSearch)
            await GoAsync();
    }

    private async Task GoAsync()
    {
        await _form.Validate();

        if (_form.IsValid)
        {
            _isBusy = true;
            _searchResult = null;

            var result = await _mediator.Send(new SearchQuery { Model = _model });

            if (result.Succeeded)
            {
                _searchResult = result.Data;
                _snackBar.Add("Search has been performed successfully!", Severity.Success, (options) => options.CloseAfterNavigation = true);
            }
            else
            {
                _snackBar.Add("Search has failed!", Severity.Error);
                foreach (var message in result.Messages)
                {
                    _snackBar.Add(message, Severity.Error, (options) => options.CloseAfterNavigation = true);
                }
            }

            _isBusy = false;
        }
    }
}