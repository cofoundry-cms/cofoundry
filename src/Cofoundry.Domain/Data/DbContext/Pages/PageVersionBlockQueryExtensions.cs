namespace Cofoundry.Domain.Data;

public static class PageVersionBlockQueryExtensions
{
    /// <summary>
    /// Filters the collection to only include blocks that
    /// have not been archived.
    /// </summary>
    public static IQueryable<PageVersionBlock> FilterActive(this IQueryable<PageVersionBlock> pages)
    {
        var filtered = pages.Where(p => !p.PageBlockType.IsArchived);

        return filtered;
    }
}
