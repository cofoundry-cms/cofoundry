namespace Cofoundry.Domain.Data;

/// <summary>
/// Extension methods for <see cref="IQueryable{PageTemplate}"/>.
/// </summary>
public static class PageTemplateQueryExtensions
{
    extension(IQueryable<PageTemplate> pageTemplates)
    {
        /// <summary>
        /// Fitlers the collection to only include templates with the 
        /// specified id.
        /// </summary>
        /// <param name="pageTemplateId">PageTemplateId to filter by.</param>
        public IQueryable<PageTemplate> FilterByPageTemplateId(int pageTemplateId)
        {
            var result = pageTemplates
                .Where(i => i.PageTemplateId == pageTemplateId);

            return result;
        }

        /// <summary>
        /// Filters the collection to only include templates that are
        /// not archived.
        /// </summary>
        public IQueryable<PageTemplate> FilterActive()
        {
            var filtered = pageTemplates.Where(p => !p.IsArchived);

            return filtered;
        }
    }
}
