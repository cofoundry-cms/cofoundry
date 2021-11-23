using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Domain.Data
{
    public static class UnstructuredDataDependencyExtensions
    {
        /// <summary>
        /// Filters the collection to only include dependencies related to the specified
        /// root entity i.e. the entity itself which may contain references to other entities.
        /// </summary>
        /// <param name="dependencies">Collection to filter.</param>
        /// <param name="rootEntityEntityDefinitionCode">
        /// 6 character identifier of the root entity type to filter to.
        /// </param>
        /// <param name="rootEntityId">Database id of the root entity to filter on.</param>
        public static IQueryable<UnstructuredDataDependency> FilterByRootEntity(this IQueryable<UnstructuredDataDependency> dependencies, string rootEntityEntityDefinitionCode, int rootEntityId)
        {
            var filtered = dependencies.Where(d => d.RootEntityDefinitionCode == rootEntityEntityDefinitionCode && d.RootEntityId == rootEntityId);

            return filtered;
        }

        /// <summary>
        /// Filters the collection to only include dependencies related to the specified
        /// root entity i.e. the entity itself which may contain references to other entities.
        /// </summary>
        /// <param name="dependencies">Collection to filter.</param>
        /// <param name="rootEntityEntityDefinitionCode">
        /// 6 character identifier of the root entity type to filter to.
        /// </param>
        /// <param name="rootEntityId">Database id of the root entity to filter on.</param>
        public static IEnumerable<UnstructuredDataDependency> FilterByRootEntity(this IEnumerable<UnstructuredDataDependency> dependencies, string rootEntityEntityDefinitionCode, int rootEntityId)
        {
            var filtered = dependencies.Where(d => d.RootEntityDefinitionCode == rootEntityEntityDefinitionCode && d.RootEntityId == rootEntityId);

            return filtered;
        }

        /// <summary>
        /// Filters the collection to only include dependencies where the specified
        /// entity is the relation i.e. the entities it is assigned to in data properties.
        /// </summary>
        /// <param name="dependencies">Collection to filter.</param>
        /// <param name="relatedEntityEntityDefinitionCode">
        /// 6 character identifier of the related entity type to filter to.
        /// </param>
        /// <param name="relatedEntityId">Database id of the related entity to filter on.</param>
        public static IQueryable<UnstructuredDataDependency> FilterByRelatedEntity(this IQueryable<UnstructuredDataDependency> dependencies, string rootEntityEntityDefinitionCode, int relatedEntityId)
        {
            var filtered = dependencies.Where(d => d.RelatedEntityDefinitionCode == rootEntityEntityDefinitionCode && d.RelatedEntityId == relatedEntityId);

            return filtered;
        }

        /// <summary>
        /// Filters the collection to only include dependencies where the specified
        /// entity is the relation i.e. the entities it is assigned to in data properties.
        /// </summary>
        /// <param name="dependencies">Collection to filter.</param>
        /// <param name="relatedEntityEntityDefinitionCode">
        /// 6 character identifier of the related entity type to filter to.
        /// </param>
        /// <param name="relatedEntityId">Database id of the related entity to filter on.</param>
        public static IEnumerable<UnstructuredDataDependency> FilterByRelatedEntity(this IEnumerable<UnstructuredDataDependency> dependencies, string relatedEntityEntityDefinitionCode, int relatedEntityId)
        {
            var filtered = dependencies.Where(d => d.RelatedEntityDefinitionCode == relatedEntityEntityDefinitionCode && d.RelatedEntityId == relatedEntityId);

            return filtered;
        }

        /// <summary>
        /// Filters the collection to only include dependencies where the specified
        /// entities are the relation i.e. the entities it is assigned to in data properties.
        /// </summary>
        /// <param name="dependencies">Collection to filter.</param>
        /// <param name="relatedEntityEntityDefinitionCode">
        /// 6 character identifier of the related entity type to filter to.
        /// </param>
        /// <param name="relatedEntityIsd">Database ids of the related entities to filter on.</param>
        public static IQueryable<UnstructuredDataDependency> FilterByRelatedEntity(this IQueryable<UnstructuredDataDependency> dependencies, string relatedEntityEntityDefinitionCode, ICollection<int> relatedEntityIds)
        {
            var filtered = dependencies.Where(d => d.RelatedEntityDefinitionCode == relatedEntityEntityDefinitionCode && relatedEntityIds.Contains(d.RelatedEntityId));

            return filtered;
        }

        /// <summary>
        /// Filters the collection to only include dependencies where the specified
        /// entities are the relation i.e. the entities it is assigned to in data properties.
        /// </summary>
        /// <param name="dependencies">Collection to filter.</param>
        /// <param name="relatedEntityEntityDefinitionCode">
        /// 6 character identifier of the related entity type to filter to.
        /// </param>
        /// <param name="relatedEntityIds">Database ids of the related entities to filter on.</param>
        public static IEnumerable<UnstructuredDataDependency> FilterByRelatedEntity(this IEnumerable<UnstructuredDataDependency> dependencies, string relatedEntityEntityDefinitionCode, ICollection<int> relatedEntityIds)
        {
            var filtered = dependencies.Where(d => d.RelatedEntityDefinitionCode == relatedEntityEntityDefinitionCode &&  relatedEntityIds.Contains(d.RelatedEntityId));

            return filtered;
        }
    }
}
