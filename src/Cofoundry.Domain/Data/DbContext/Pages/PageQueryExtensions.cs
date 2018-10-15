using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    public static class PageQueryExtensions
    {
        /// <summary>
        /// Fitlers the collection to only include pages with the 
        /// specified page id.
        /// </summary>
        /// <param name="pageId">PageId to filter by.</param>
        public static IQueryable<Page> FilterByPageId(this IQueryable<Page> pages, int pageId)
        {
            var result = pages
                .Where(i => i.PageId == pageId);

            return result;
        }

        /// <summary>
        /// Filters the collection to only include versions that are
        /// not deleted and not in deleted directories.
        /// </summary>
        public static IQueryable<Page> FilterActive(this IQueryable<Page> pages)
        {
            // Not currently filtered, but may need to add locale filtering in here
            // in a later version so will leave this here for now.
            return pages;
        }
    }
}
