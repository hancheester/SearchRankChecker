using SearchRankChecker.Application.Search.Dto;

namespace SearchRankChecker.Client.Store.Search.Actions;

public class SetSearchDtoAction
{
    public SearchDto Model { get; set; }

    public SetSearchDtoAction(SearchDto dto)
    {
        Model = dto;
    }
}
