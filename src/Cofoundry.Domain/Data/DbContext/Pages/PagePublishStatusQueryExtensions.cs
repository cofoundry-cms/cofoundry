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
        /// <param name="publishStatusQuery">Query status to filter by. If the value is PublishStatusQuery.Published then additional filtering by publish date will be applied.</param>
        /// <param name="executionDate">UTC execution date of the query. This is used to compare the publish date.</param>
        public static IQueryable<PagePublishStatusQuery> FilterByStatus(this IQueryable<PagePublishStatusQuery> source, PublishStatusQuery publishStatusQuery, DateTime executionDate)
        {
            if (publishStatusQuery == PublishStatusQuery.SpecificVersion)
            {
                throw new Exception("Cannot filter by PublishStatusQuery.SpecificVersion using the FilterByStatus extension method.");
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
        public static IQueryable<PagePublishStatusQuery> FilterActive(this IQueryable<PagePublishStatusQuery> source)
        {
            var filtered = source
                .Where(p => !p.PageVersion.PageTemplate.IsArchived);

            return filtered;
        }

        /// <summary>
        /// Filters the collection to only include records with the specified page id.
        /// </summary>
        public static IQueryable<PagePublishStatusQuery> FilterByPageId(this IQueryable<PagePublishStatusQuery> source, int pageId)
        {
            var filtered = source
                .Where(p => p.PageId == pageId);

            return filtered;
        }

        /// <summary>
        /// Filters the collection to only include records with the specified locale id.
        /// </summary>
        public static IQueryable<PagePublishStatusQuery> FilterByLocaleId(this IQueryable<PagePublishStatusQuery> source, int localeId)
        {
            var filtered = source
                .Where(p => p.Page.LocaleId == localeId);

            return filtered;
        }

        /// <summary>
        /// Filters the collection to only include records with the specified page directory id.
        /// </summary>
        public static IQueryable<PagePublishStatusQuery> FilterByDirectoryId(this IQueryable<PagePublishStatusQuery> source, int directoryId)
        {
            var filtered = source
                .Where(p => p.Page.PageDirectoryId == directoryId);

            return filtered;
        }


        /// <summary>
        /// Sorts the collection using the standard query sort parameters and rules.
        /// </summary>
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
                        .OrderByWithSortDirection(e => e.Page.Locale.IETFLanguageTag, sortDirection)
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
}
