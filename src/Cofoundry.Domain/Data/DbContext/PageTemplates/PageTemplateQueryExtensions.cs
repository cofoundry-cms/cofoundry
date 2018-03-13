using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    public static class PageTemplateQueryExtensions
    {
        /// <summary>
        /// Fitlers the collection to only include templates with the 
        /// specified id.
        /// </summary>
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
        public static IQueryable<PageTemplate> FilterActive(this IQueryable<PageTemplate> pageTemplates)
        {
            var filtered = pageTemplates.Where(p => !p.IsArchived);

            return filtered;
        }
    }
}
