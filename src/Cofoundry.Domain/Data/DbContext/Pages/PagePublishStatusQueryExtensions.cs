using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    public static class PagePublishStatusQueryExtensions
    {
        /// <summary>
        /// Filters the results by the publish status query type.
        /// </summary>
        /// <param name="statusQuery">Query status to filter by. If the value is PublishStatusQuery.Published then additional filtering by publish date will be applied.</param>
        /// <param name="executionDate">UTC execution date of the query. This is used to compare the publish date.</param>
        public static IQueryable<PagePublishStatusQuery> FilterByStatus(this IQueryable<PagePublishStatusQuery> source, PublishStatusQuery statusQuery, DateTime executionDate)
        {
            if (statusQuery == PublishStatusQuery.SpecificVersion)
            {
                throw new Exception("Cannot filter by PublishStatusQuery.SpecificVersion using the FilterByStatus extension method.");
            }

            IQueryable<PagePublishStatusQuery> filtered;

            if (statusQuery == PublishStatusQuery.Published)
            {
                filtered = source
                    .Where(p => p.PublishStatusQueryId == (short)statusQuery && p.Page.PublishDate <= executionDate);
            }
            else
            {
                filtered = source
                    .Where(p => p.PublishStatusQueryId == (short)statusQuery);
            }

            return filtered;
        }

        /// <summary>
        /// Filters the collection to only include pages that are
        /// not deleted.
        /// </summary>
        public static IQueryable<PagePublishStatusQuery> FilterActive(this IQueryable<PagePublishStatusQuery> source)
        {
            var filtered = source
                .Where(p => !p.PageVersion.IsDeleted 
                    && !p.Page.IsDeleted 
                    && !p.PageVersion.PageTemplate.IsArchived 
                    && p.Page.PageDirectory.IsActive
                    );

            return filtered;
        }

        /// <summary>
        /// Filters the collection to only include records with the specifed PageId.
        /// </summary>
        public static IQueryable<PagePublishStatusQuery> FilterByPageId(this IQueryable<PagePublishStatusQuery> source, int pageId)
        {
            var filtered = source
                .Where(p => p.PageId == pageId);

            return filtered;
        }
    }
}
