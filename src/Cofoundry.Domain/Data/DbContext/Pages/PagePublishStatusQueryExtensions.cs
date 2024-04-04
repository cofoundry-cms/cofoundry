namespace Cofoundry.Domain.Data;

public static class PagePublishStatusQueryExtensions
{
    /// <summary>
    /// Filters the results by the publish status query type.
    /// </summary>
    /// <param name="source">
    /// Queryable instance to filter.
    /// </param>
    /// <param name="publishStatusQuery">Query status to filter by. If the value is PublishStatusQuery.Published then additional filtering by publish date will be applied.</param>
    /// <param name="executionDate">UTC execution date of the query. This is used to compare the publish date.</param>
    public static IQueryable<PagePublishStatusQuery> FilterByStatus(this IQueryable<PagePublishStatusQuery> source, PublishStatusQuery publishStatusQuery, DateTime executionDate)
    {
        if (publishStatusQuery == PublishStatusQuery.SpecificVersion)
        {
            throw new InvalidOperationException($"Cannot filter by {nameof(PublishStatusQuery)}.{nameof(PublishStatusQuery.SpecificVersion)} using the {nameof(FilterByStatus)} extension method.");
        }

        IQueryable<PagePublishStatusQuery> filtered;

        if (publishStatusQuery == PublishStatusQuery.Published)
        {
            filtered = source
                .Where(p => p.PublishStatusQueryId == (short)publishStatusQuery && p.Page.PublishDate <= executionDate);
        }
        else
        {
            filtered = source
                .Where(p => p.PublishStatusQueryId == (short)publishStatusQuery);
        }

        return filtered;
    }

    /// <summary>
    /// Filters the collection to only include pages that are
    /// not deleted.
    /// </summary>
    /// <param name="source">
    /// Queryable instance to filter.
    /// </param>
    public static IQueryable<PagePublishStatusQuery> FilterActive(this IQueryable<PagePublishStatusQuery> source)
    {
        var filtered = source
            .Where(p => !p.PageVersion.PageTemplate.IsArchived);

        return filtered;
    }

    /// <summary>
    /// Filters the collection to only include records with the specified page id.
    /// </summary>
    /// <param name="source">
    /// Queryable instance to filter.
    /// </param>
    /// <param name="pageId">
    /// Id of the page to filter by.
    /// </param>
    public static IQueryable<PagePublishStatusQuery> FilterByPageId(this IQueryable<PagePublishStatusQuery> source, int pageId)
    {
        var filtered = source
            .Where(p => p.PageId == pageId);

        return filtered;
    }

    /// <summary>
    /// Filters the collection to only include records with the specified locale id.
    /// </summary>
    /// <param name="source">
    /// Queryable instance to filter.
    /// </param>
    /// <param name="localeId">
    /// Id of the locale to filter by.
    /// </param>
    public static IQueryable<PagePublishStatusQuery> FilterByLocaleId(this IQueryable<PagePublishStatusQuery> source, int localeId)
    {
        var filtered = source
            .Where(p => p.Page.LocaleId == localeId);

        return filtered;
    }

    /// <summary>
    /// Filters the collection to only include records with the specified page directory id.
    /// </summary>
    /// <param name="source">
    /// Queryable instance to filter.
    /// </param>
    /// <param name="directoryId">
    /// Id of the directory to filter by.
    /// </param>
    public static IQueryable<PagePublishStatusQuery> FilterByDirectoryId(this IQueryable<PagePublishStatusQuery> source, int directoryId)
    {
        var filtered = source
            .Where(p => p.Page.PageDirectoryId == directoryId);

        return filtered;
    }

    /// <summary>
    /// Sorts the collection using the standard query sort parameters and rules.
    /// </summary>
    /// <param name="source">
    /// Queryable instance to sort.
    /// </param>
    /// <param name="pageQuerySortType">Field to sort on.</param>
    /// <param name="sortDirection">Direction to sort by.</param>
    public static IOrderedQueryable<PagePublishStatusQuery> SortBy(
        this IQueryable<PagePublishStatusQuery> source,
        PageQuerySortType pageQuerySortType,
        SortDirection sortDirection
        )
    {
        IOrderedQueryable<PagePublishStatusQuery> result;

        switch (pageQuerySortType)
        {
            case PageQuerySortType.Default:
            case PageQuerySortType.Title:
                result = source
                    .OrderByWithSortDirection(e => e.PageVersion.Title, sortDirection);
                break;
            case PageQuerySortType.Locale:
                result = source
                    .OrderByWithSortDirection(e => e.Page.Locale!.IETFLanguageTag, sortDirection)
                    .ThenByWithSortDirection(e => e.PageVersion.Title, sortDirection);
                break;
            case PageQuerySortType.CreateDate:
                result = source
                    .OrderByDescendingWithSortDirection(e => e.PageVersion.CreateDate, sortDirection);
                break;
            case PageQuerySortType.PublishDate:
                result = source
                    .OrderByDescendingWithSortDirection(e => e.Page.PublishDate ?? e.PageVersion.CreateDate, sortDirection)
                    ;
                break;
            default:
                throw new Exception($"{nameof(PageQuerySortType)} not recognised.");
        }

        return result;
    }
}
