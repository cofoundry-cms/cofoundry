namespace Cofoundry.Domain.Data;

public static class PageTemplateQueryExtensions
{
    /// <summary>
    /// Fitlers the collection to only include templates with the 
    /// specified id.
    /// </summary>
    /// <param name="pageTemplates">
    /// Queryable instance to filter.
    /// </param>
    /// <param name="pageTemplateId">PageTemplateId to filter by.</param>
    public static IQueryable<PageTemplate> FilterByPageTemplateId(this IQueryable<PageTemplate> pageTemplates, int pageTemplateId)
    {
        var result = pageTemplates
            .Where(i => i.PageTemplateId == pageTemplateId);

        return result;
    }

    /// <summary>
    /// Filters the collection to only include templates that are
    /// not archived.
    /// </summary>
    /// <param name="pageTemplates">
    /// Queryable instance to filter.
    /// </param>
    public static IQueryable<PageTemplate> FilterActive(this IQueryable<PageTemplate> pageTemplates)
    {
        var filtered = pageTemplates.Where(p => !p.IsArchived);

        return filtered;
    }
}
