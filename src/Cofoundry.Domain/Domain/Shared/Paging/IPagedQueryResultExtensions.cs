namespace Cofoundry.Domain;

public static class IPagedQueryResultExtensions
{
    public static bool IsFirstPage(this IPagedQueryResult source)
    {
        return source.PageNumber <= 1;
    }

    public static bool IsLastPage(this IPagedQueryResult source)
    {
        return source.PageCount <= source.PageNumber;
    }
}