namespace SearchRankChecker.Application.Common.Routes;

public static class SearchApiEndpoints
{
    public static string Search => "api/v1/search";
    public static string History => "api/v1/search/history";
    public static string DeleteHistory(int id) => $"api/v1/search/history/{id}";
}
