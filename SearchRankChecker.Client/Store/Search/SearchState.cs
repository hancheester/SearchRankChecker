using Fluxor;
using SearchRankChecker.Application.Search.Dto;

namespace SearchRankChecker.Client.Store.Search;

[FeatureState]
public class SearchState
{
    public SearchDto Model { get; init; }

    public SearchState()
    {
        Model = new();
    }
}
