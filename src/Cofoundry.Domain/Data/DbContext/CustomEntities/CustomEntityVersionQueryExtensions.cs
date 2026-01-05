namespace Cofoundry.Domain.Data;

/// <summary>
/// Extension methods for <see cref="IQueryable{CustomEntityVersion}"/>.
/// </summary>
public static class CustomEntityVersionQueryExtensions
{
    extension(IQueryable<CustomEntityVersion> customEntities)
    {
        /// <summary>
        /// Filters the collection to only include records associated with
        /// the specified CustomEntityId.
        /// </summary>
        /// <param name="customEntityId">Database id of the entity to filter by.</param>
        public IQueryable<CustomEntityVersion> FilterByCustomEntityId(int customEntityId)
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
        public IQueryable<CustomEntityVersion> FilterByCustomEntityUrlSlug(string urlSlug)
        {
            var result = customEntities
                .Where(e => e.CustomEntity.UrlSlug == urlSlug);

            return result;
        }

        /// <summary>
        /// Filters the collection to only include the version with the specific Id
        /// </summary>
        /// <param name="customEntityVersionId">Database id of the version to filter by.</param>
        public IQueryable<CustomEntityVersion> FilterByCustomEntityVersionId(int customEntityVersionId)
        {
            var result = customEntities
                .Where(e => e.CustomEntityVersionId == customEntityVersionId);

            return result;
        }

        /// <summary>
        /// Fitlers the results to only include custom entities of a specific type.
        /// </summary>
        /// <param name="customEntityDefinitionCode">Unique definition code of the custom entity type to filter by.</param>
        public IQueryable<CustomEntityVersion> FilterByCustomEntityDefinitionCode(string customEntityDefinitionCode)
        {
            var result = customEntities
                .Where(e => e.CustomEntity.CustomEntityDefinitionCode == customEntityDefinitionCode);

            return result;
        }

        /// <summary>
        /// Removes any custom entities from the query that are inactive (attached to an inactive locale).
        /// </summary>
        public IQueryable<CustomEntityVersion> FilterActive()
        {
            var result = customEntities
                .Where(e => e.CustomEntity.LocaleId == null || e.CustomEntity.Locale!.IsActive);

            return result;
        }

        public IQueryable<CustomEntityVersion> FilterByLocale(int? localeId)
        {
            var result = customEntities
                .Where(e => e.CustomEntity.LocaleId == localeId);

            return result;
        }
    }
}
