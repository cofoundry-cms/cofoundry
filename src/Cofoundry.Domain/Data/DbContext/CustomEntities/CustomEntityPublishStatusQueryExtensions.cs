using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    public static class CustomEntityPublishStatusQueryExtensions
    {
        /// <summary>
        /// Filters the results by the publish status query type.
        /// </summary>
        /// <param name="statusQuery">Query status to filter by. If the value is PublishStatusQuery.Published then additional filtering by publish date will be applied.</param>
        /// <param name="executionDate">UTC execution date of the query. This is used to compare the publish date.</param>
        public static IQueryable<CustomEntityPublishStatusQuery> FilterByStatus(this IQueryable<CustomEntityPublishStatusQuery> source, PublishStatusQuery statusQuery, DateTime executionDate)
        {
            if (statusQuery == PublishStatusQuery.SpecificVersion)
            {
                throw new Exception("Cannot filter by PublishStatusQuery.SpecificVersion using the FilterByStatus extension method.");
            }

            IQueryable<CustomEntityPublishStatusQuery> filtered;

            if (statusQuery == PublishStatusQuery.Published)
            {
                filtered = source
                    .Where(p => p.PublishStatusQueryId == (short)statusQuery && p.CustomEntity.PublishDate <= executionDate);
            }
            else
            {
                filtered = source
                    .Where(p => p.PublishStatusQueryId == (short)statusQuery);
            }

            return filtered;
        }

        /// <summary>
        /// Removes any custom entities from the query that are inactive (attached to an inactive locale).
        /// </summary>
        public static IQueryable<CustomEntityPublishStatusQuery> FilterActive(this IQueryable<CustomEntityPublishStatusQuery> customEntities)
        {
            var result = customEntities
                .Where(e => e.CustomEntity.LocaleId == null || e.CustomEntity.Locale.IsActive);

            return result;
        }

        /// <summary>
        /// Filters the collection to only include records associated with
        /// the specified CustomEntityId.
        /// </summary>
        /// <param name="customEntityId">Database id of the entity to filter by.</param>
        public static IQueryable<CustomEntityPublishStatusQuery> FilterByCustomEntityId(this IQueryable<CustomEntityPublishStatusQuery> source, int customEntityId)
        {
            var filtered = source
                .Where(e => e.CustomEntityId == customEntityId);

            return filtered;
        }

        /// <summary>
        /// Filters the collection to only include records associated with
        /// the specified custom entity UrlSlug.
        /// </summary>
        /// <param name="urlSlug">Url slug of the custom entity record to filter by.</param>
        public static IQueryable<CustomEntityPublishStatusQuery> FilterByCustomEntityUrlSlug(this IQueryable<CustomEntityPublishStatusQuery> customEntities, string urlSlug)
        {
            var result = customEntities
                .Where(e => e.CustomEntity.UrlSlug == urlSlug);

            return result;
        }

        /// <summary>
        /// Fitlers the results to only include custom entities of a specific type.
        /// </summary>
        /// <param name="customEntityDefinitionCode">Unique definition code of the custom entity type to filter by.</param>
        public static IQueryable<CustomEntityPublishStatusQuery> FilterByCustomEntityDefinitionCode(this IQueryable<CustomEntityPublishStatusQuery> customEntities, string customEntityDefinitionCode)
        {
            var result = customEntities
                .Where(e => e.CustomEntity.CustomEntityDefinitionCode == customEntityDefinitionCode);

            return result;
        }

        /// <summary>
        /// Applies the specified sorting to the query, taking into account
        /// the any default sorting settings on the custom entity definition.
        /// </summary>
        /// <param name="dbQuery">Query to sort.</param>
        /// <param name="customEntityDefinition">
        /// The definition for the entity being sorted. This may contain default
        /// sorting settings that will be applied if CustomEntityQuerySortType.Default
        /// is specified.
        /// </param>
        /// <param name="sortType">The sort type to apply.</param>
        /// <param name="optionalSortDirection">
        /// An optional sort direction, falling back to the default
        /// if not specified.
        /// </param>
        public static IOrderedQueryable<CustomEntityPublishStatusQuery> SortBy(
            this IQueryable<CustomEntityPublishStatusQuery> dbQuery,
            ICustomEntityDefinition customEntityDefinition,
            CustomEntityQuerySortType sortType, 
            SortDirection? optionalSortDirection = null
            )
        {
            IOrderedQueryable<CustomEntityPublishStatusQuery> result;

            var sortDirection = optionalSortDirection ?? SortDirection.Default;

            if (sortType == CustomEntityQuerySortType.Default 
                && customEntityDefinition is ISortedCustomEntityDefinition sortedDefinition
                && !(customEntityDefinition is IOrderableCustomEntityDefinition))
            {
                sortType = sortedDefinition.DefaultSortType;
                if (!optionalSortDirection.HasValue)
                {
                    sortDirection = sortedDefinition.DefaultSortDirection;
                }
            }

            switch (sortType)
            {
                case CustomEntityQuerySortType.Default:
                case CustomEntityQuerySortType.Natural:
                    if (UseOrderableSort(customEntityDefinition))
                    {
                        result = SortByOrdering(dbQuery, sortDirection, customEntityDefinition);
                    }
                    else
                    {
                        result = dbQuery
                            .OrderByWithSortDirection(e => e.CustomEntityVersion.Title, sortDirection);
                    }
                    break;
                case CustomEntityQuerySortType.Locale:

                    if (UseOrderableSort(customEntityDefinition))
                    {
                        result = SortByOrdering(dbQuery, sortDirection, customEntityDefinition);
                    }
                    else
                    {
                        result = dbQuery
                            .OrderByWithSortDirection(e => e.CustomEntity.Locale.IETFLanguageTag, sortDirection)
                            .ThenByWithSortDirection(e => e.CustomEntityVersion.Title, sortDirection);
                    }
                    break;
                case CustomEntityQuerySortType.Title:
                    result = dbQuery
                        .OrderByWithSortDirection(e => e.CustomEntityVersion.Title, sortDirection);
                    break;
                case CustomEntityQuerySortType.CreateDate:
                    result = dbQuery
                        .OrderByDescendingWithSortDirection(e => e.CustomEntity.CreateDate, sortDirection);
                    break;
                case CustomEntityQuerySortType.PublishDate:
                    result = dbQuery
                        .OrderByDescendingWithSortDirection(e => e.CustomEntity.PublishDate ?? e.CustomEntityVersion.CreateDate, sortDirection)
                        ;
                    break;
                default:
                    throw new Exception($"{nameof(CustomEntityQuerySortType)} not recognised: {sortType}");
            }

            return result;
        }

        private static IOrderedQueryable<CustomEntityPublishStatusQuery> SortByOrdering(
            IQueryable<CustomEntityPublishStatusQuery> dbQuery,
            SortDirection sortDirection,
            ICustomEntityDefinition definition
            )
        {

            var primarySorted = dbQuery
                .OrderByWithSortDirection(e => e.CustomEntity.Locale.IETFLanguageTag, sortDirection)
                .ThenByWithSortDirection(e => !e.CustomEntity.Ordering.HasValue, sortDirection)
                .ThenByWithSortDirection(e => e.CustomEntity.Ordering, sortDirection);

            // Partially ordered entities will need a secondary sort level defined
            if (definition is ISortedCustomEntityDefinition sortedDefinition)
            {
                // secondary sort direction can be different
                if (sortDirection == SortDirection.Default)
                {
                    sortDirection = sortedDefinition.DefaultSortDirection;
                }
                else
                {
                    // Flip the secondary sort direction if the requested sort direction is reversed
                    sortDirection = sortedDefinition.DefaultSortDirection == SortDirection.Default ? SortDirection.Reversed : SortDirection.Default;
                }

                switch (sortedDefinition.DefaultSortType)
                {
                    case CustomEntityQuerySortType.Default:
                    case CustomEntityQuerySortType.Natural:
                    case CustomEntityQuerySortType.Title:
                        return primarySorted.ThenByWithSortDirection(e => e.CustomEntityVersion.Title, sortDirection);
                    case CustomEntityQuerySortType.Locale:
                        return primarySorted
                                .ThenByWithSortDirection(e => e.CustomEntity.Locale.IETFLanguageTag, sortDirection)
                                .ThenByWithSortDirection(e => e.CustomEntityVersion.Title, sortDirection);

                    case CustomEntityQuerySortType.CreateDate:
                        return primarySorted.ThenByDescendingWithSortDirection(e => e.CustomEntity.CreateDate, sortDirection);

                    case CustomEntityQuerySortType.PublishDate:
                        return primarySorted.ThenByDescendingWithSortDirection(e => e.CustomEntity.PublishDate ?? e.CustomEntityVersion.CreateDate, sortDirection);
                    default:
                        throw new Exception($"{nameof(CustomEntityQuerySortType)} not recognised: {sortedDefinition.DefaultSortType}");
                }
            }

            // default secondary sorting
            return primarySorted.ThenByWithSortDirection(e => e.CustomEntityVersion.Title, sortDirection);
        }

        private static bool UseOrderableSort(ICustomEntityDefinition customEntityDefinition)
        {
            return customEntityDefinition is IOrderableCustomEntityDefinition orderableCustomEntityDefinition
                        && orderableCustomEntityDefinition.Ordering != CustomEntityOrdering.None;
        }
    }
}
