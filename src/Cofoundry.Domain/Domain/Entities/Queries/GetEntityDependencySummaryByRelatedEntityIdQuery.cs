using Cofoundry.Domain.CQS;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Returns information about entities that have a dependency on the entity 
    /// being queried, typically becuase they reference the entity in an 
    /// unstructured data blob where the relationship cannot be enforced by
    /// the database.
    /// </summary>
    public class GetEntityDependencySummaryByRelatedEntityIdQuery : IQuery<ICollection<EntityDependencySummary>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetEntityDependencySummaryByRelatedEntityIdQuery"/> class.
        /// </summary>
        public GetEntityDependencySummaryByRelatedEntityIdQuery()
        {
        }

        /// <summary>
        /// Initializes a new instance with query parameters.
        /// </summary>
        /// <param name="entityDefinitionCode">
        /// <see cref="IEntityDefinition.EntityDefinitionCode"/> of the entity type
        /// to check.
        /// </param>
        /// <param name="entityIds">
        /// Database id of the entity to check for required dependencies. Each entity 
        /// must be of the type specified by <paramref name="entityDefinitionCode"/>.
        /// </param>
        public GetEntityDependencySummaryByRelatedEntityIdQuery(string entityDefinitionCode, int entityId)
        {
            EntityDefinitionCode = entityDefinitionCode;
            EntityId = entityId;
        }

        /// <summary>
        /// <see cref="IEntityDefinition.EntityDefinitionCode"/> of the entity type
        /// to check.
        /// </summary>
        [Required]
        public string EntityDefinitionCode { get; set; }

        /// <summary>
        /// Database id of the entity to check for required dependencies. Each entity 
        /// must be of the type specified by <see cref="EntityDefinitionCode"/>.
        /// </summary>
        public int EntityId { get; set; }

        /// <summary>
        /// If set to true, then deletable dependencies will be excluded
        /// from the results, leaving only those relations that are required
        /// and cannot be removed i.e. those that would prevent the entity
        /// from being deleted.
        /// </summary>
        public bool ExcludeDeletable { get; set; }
    }
}
