using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    public static class CustomEntityVersionQueryExtensions
    {
        /// <summary>
        /// Filters the collection to only include records associated with
        /// the specified CustomEntityId.
        /// </summary>
        /// <param name="customEntityId">Database id of the entity to filter by.</param>
        public static IQueryable<CustomEntityVersion> FilterByCustomEntityId(this IQueryable<CustomEntityVersion> customEntities, int customEntityId)
        {
            var result = customEntities
                .Where(e => e.CustomEntityId == customEntityId);

            return result;
        }

        /// <summary>
        /// Filters the collection to only include records associated with
        /// the specified custom entity UrlSlug.
        /// </summary>
        /// <param name="urlSlug">Url slug of the custom entity record to filter by.</param>
        public static IQueryable<CustomEntityVersion> FilterByCustomEntityUrlSlug(this IQueryable<CustomEntityVersion> customEntities, string urlSlug)
        {
            var result = customEntities
                .Where(e => e.CustomEntity.UrlSlug == urlSlug);

            return result;
        }

        /// <summary>
        /// Filters the collection to only include the version with the specific Id
        /// </summary>
        /// <param name="customEntityVersionId">Database id of the version to filter by.</param>
        public static IQueryable<CustomEntityVersion> FilterByCustomEntityVersionId(this IQueryable<CustomEntityVersion> customEntities, int customEntityVersionId)
        {
            var result = customEntities
                .Where(e => e.CustomEntityVersionId == customEntityVersionId);

            return result;
        }

        /// <summary>
        /// Fitlers the results to only include custom entities of a specific type.
        /// </summary>
        /// <param name="customEntityDefinitionCode">Unique definition code of the custom entity type to filter by.</param>
        public static IQueryable<CustomEntityVersion> FilterByCustomEntityDefinitionCode(this IQueryable<CustomEntityVersion> customEntities, string customEntityDefinitionCode)
        {
            var result = customEntities
                .Where(e => e.CustomEntity.CustomEntityDefinitionCode == customEntityDefinitionCode);

            return result;
        }

        /// <summary>
        /// Removes any custom entities from the query that are inactive (attached to an inactive locale).
        /// </summary>
        public static IQueryable<CustomEntityVersion> FilterActive(this IQueryable<CustomEntityVersion> customEntities)
        {
            var result = customEntities
                .Where(e => e.CustomEntity.LocaleId == null || e.CustomEntity.Locale.IsActive);

            return result;
        }

        public static IQueryable<CustomEntityVersion> FilterByLocale(this IQueryable<CustomEntityVersion> customEntities, int? localeId)
        {
            var result = customEntities
                .Where(e => e.CustomEntity.LocaleId == localeId);

            return result;
        }
    }
}
