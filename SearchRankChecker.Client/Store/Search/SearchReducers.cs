using Fluxor;
using SearchRankChecker.Client.Store.Search.Actions;

namespace SearchRankChecker.Client.Store.Search;

public static class SearchReducers
{
    [ReducerMethod]
    public static SearchState ReduceResetSearchAction(SearchState state, ResetSearchAction _) => new();

    [ReducerMethod]
    public static SearchState ReduceSetSearchDtoAction(SearchState state, SetSearchDtoAction action) =>
        state with
        {
            Model = action.Model
        };

    [ReducerMethod]
    public static SearchState ReduceAutoSearchAction(SearchState state, AutoSearchAction action) => 
        state with
        {
            AutoSearch = true
        };
}