namespace Cofoundry.Domain.Data;

public static class CustomEntityVersionPageBlockQueryExtensions
{
    /// <summary>
    /// Filters the collection to only include blocks that
    /// have not been archived.
    /// </summary>
    public static IQueryable<CustomEntityVersionPageBlock> FilterActive(this IQueryable<CustomEntityVersionPageBlock> pages)
    {
        var filtered = pages.Where(p => !p.PageBlockType.IsArchived);

        return filtered;
    }
}
