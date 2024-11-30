using Fluxor;
using SearchRankChecker.Client.Store.Search.Actions;

namespace SearchRankChecker.Client.Store.Search;

public static class SearchReducers
{
    [ReducerMethod]
    public static SearchState ReduceResetSearchAction(SearchState state, ResetSearchAction _) => new();

    [ReducerMethod]
    public static SearchState ReduceSetSearchDtoAction(SearchState state, SetSearchDtoAction action) =>
        new()
        {
            Model = action.Model
        };
}