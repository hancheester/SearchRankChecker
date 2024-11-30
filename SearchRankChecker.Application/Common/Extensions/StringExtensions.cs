namespace SearchRankChecker.Application.Common.Extensions;

public static class StringExtensions
{
    public static string EnsureEndsWith(this string s, string value, StringComparison comparisonType = StringComparison.Ordinal)
    {
        if (!string.IsNullOrEmpty(s) && s.EndsWith(value, comparisonType))
        {
            return s;
        }

        return s + value;
    }
}