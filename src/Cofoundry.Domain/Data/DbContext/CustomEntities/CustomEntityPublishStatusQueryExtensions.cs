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
        public static IQueryable<CustomEntityPublishStatusQuery> FilterByActive(this IQueryable<CustomEntityPublishStatusQuery> customEntities)
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
        /// Fitlers the results to only include custom entities of a specific type.
        /// </summary>
        /// <param name="customEntityDefinitionCode">Unique definition code of the custom entity type to filter by.</param>
        public static IQueryable<CustomEntityPublishStatusQuery> FilterByCustomEntityDefinitionCode(this IQueryable<CustomEntityPublishStatusQuery> customEntities, string customEntityDefinitionCode)
        {
            var result = customEntities
                .Where(e => e.CustomEntity.CustomEntityDefinitionCode == customEntityDefinitionCode);

            return result;
        }
    }
}
