using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    public static class PageVersionQueryExtensions
    {
        /// <summary>
        /// Filters the result to include only the version with the specified PageVersionId
        /// </summary>
        public static IQueryable<PageVersion> FilterByPageVersionId(this IQueryable<PageVersion> pageVersions, int pageVersionId)
        {
            var filtered = pageVersions
                .Where(v => v.PageVersionId == pageVersionId);

            return filtered;
        }

        /// <summary>
        /// Filters the result to include only the version with the specified PageVersionId
        /// </summary>
        public static IQueryable<PageVersion> FilterByPageId(this IQueryable<PageVersion> pageVersions, int pageId)
        {
            var filtered = pageVersions
                .Where(v => v.PageId == pageId);

            return filtered;
        }

        /// <summary>
        /// Filters the collection to only include versions that are
        /// not deleted.
        /// </summary>
        public static IQueryable<PageVersion> FilterActive(this IQueryable<PageVersion> pageVersions)
        {
            var filtered = pageVersions
                .Where(v => !v.PageTemplate.IsArchived);

            return filtered;
        }
    }
}
