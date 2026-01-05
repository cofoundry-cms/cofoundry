namespace Cofoundry.Domain.Data;

/// <summary>
/// Extension methods for <see cref="IQueryable{UnstructuredDataDependency}"/>.
/// </summary>
public static class UnstructuredDataDependencyExtensions
{
    extension(IQueryable<UnstructuredDataDependency> dependencies)
    {
        /// <summary>
        /// Filters the collection to only include dependencies related to the specified
        /// root entity i.e. the entity itself which may contain references to other entities.
        /// </summary>
        /// <param name="rootEntityEntityDefinitionCode">
        /// 6 character identifier of the root entity type to filter to.
        /// </param>
        /// <param name="rootEntityId">Database id of the root entity to filter on.</param>
        public IQueryable<UnstructuredDataDependency> FilterByRootEntity(string rootEntityEntityDefinitionCode, int rootEntityId)
        {
            var filtered = dependencies.Where(d => d.RootEntityDefinitionCode == rootEntityEntityDefinitionCode && d.RootEntityId == rootEntityId);

            return filtered;
        }

        /// <summary>
        /// Filters the collection to only include dependencies where the specified
        /// entity is the relation i.e. the entities it is assigned to in data properties.
        /// </summary>
        /// <param name="relatedEntityEntityDefinitionCode">
        /// 6 character identifier of the related entity type to filter to.
        /// </param>
        /// <param name="relatedEntityId">Database id of the related entity to filter on.</param>
        public IQueryable<UnstructuredDataDependency> FilterByRelatedEntity(string relatedEntityEntityDefinitionCode, int relatedEntityId)
        {
            var filtered = dependencies.Where(d => d.RelatedEntityDefinitionCode == relatedEntityEntityDefinitionCode && d.RelatedEntityId == relatedEntityId);

            return filtered;
        }

        /// <summary>
        /// Filters the collection to only include dependencies where the specified
        /// entities are the relation i.e. the entities it is assigned to in data properties.
        /// </summary>
        /// <param name="relatedEntityEntityDefinitionCode">
        /// 6 character identifier of the related entity type to filter to.
        /// </param>
        /// <param name="relatedEntityIds">Database ids of the related entities to filter on.</param>
        public IQueryable<UnstructuredDataDependency> FilterByRelatedEntity(string relatedEntityEntityDefinitionCode, IEnumerable<int> relatedEntityIds)
        {
            var filtered = dependencies.Where(d => d.RelatedEntityDefinitionCode == relatedEntityEntityDefinitionCode && relatedEntityIds.Contains(d.RelatedEntityId));

            return filtered;
        }
    }

    extension(IEnumerable<UnstructuredDataDependency> dependencies)
    {
        /// <summary>
        /// Filters the collection to only include dependencies related to the specified
        /// root entity i.e. the entity itself which may contain references to other entities.
        /// </summary>
        /// <param name="rootEntityEntityDefinitionCode">
        /// 6 character identifier of the root entity type to filter to.
        /// </param>
        /// <param name="rootEntityId">Database id of the root entity to filter on.</param>
        public IEnumerable<UnstructuredDataDependency> FilterByRootEntity(string rootEntityEntityDefinitionCode, int rootEntityId)
        {
            var filtered = dependencies.Where(d => d.RootEntityDefinitionCode == rootEntityEntityDefinitionCode && d.RootEntityId == rootEntityId);

            return filtered;
        }

        /// <summary>
        /// Filters the collection to only include dependencies where the specified
        /// entity is the relation i.e. the entities it is assigned to in data properties.
        /// </summary>
        /// <param name="relatedEntityEntityDefinitionCode">
        /// 6 character identifier of the related entity type to filter to.
        /// </param>
        /// <param name="relatedEntityId">Database id of the related entity to filter on.</param>
        public IEnumerable<UnstructuredDataDependency> FilterByRelatedEntity(string relatedEntityEntityDefinitionCode, int relatedEntityId)
        {
            var filtered = dependencies.Where(d => d.RelatedEntityDefinitionCode == relatedEntityEntityDefinitionCode && d.RelatedEntityId == relatedEntityId);

            return filtered;
        }

        /// <summary>
        /// Filters the collection to only include dependencies where the specified
        /// entities are the relation i.e. the entities it is assigned to in data properties.
        /// </summary>
        /// <param name="relatedEntityEntityDefinitionCode">
        /// 6 character identifier of the related entity type to filter to.
        /// </param>
        /// <param name="relatedEntityIds">Database ids of the related entities to filter on.</param>
        public IEnumerable<UnstructuredDataDependency> FilterByRelatedEntity(string relatedEntityEntityDefinitionCode, IEnumerable<int> relatedEntityIds)
        {
            var filtered = dependencies.Where(d => d.RelatedEntityDefinitionCode == relatedEntityEntityDefinitionCode && relatedEntityIds.Contains(d.RelatedEntityId));

            return filtered;
        }
    }
}
