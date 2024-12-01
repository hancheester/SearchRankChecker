using Fluxor;
using SearchRankChecker.Application.Search.Dto;

namespace SearchRankChecker.Client.Store.Search;

[FeatureState]
public record SearchState
{
    public SearchDto Model { get; init; }
    public bool AutoSearch { get; init; }

    public SearchState()
    {
        Model = new();
    }
}
