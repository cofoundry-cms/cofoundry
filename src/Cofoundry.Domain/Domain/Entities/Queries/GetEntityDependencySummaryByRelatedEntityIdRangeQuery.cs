namespace Cofoundry.Domain;

/// <summary>
/// Returns information about entities that have a dependency on the entity 
/// being queried, typically becuase they reference the entity in an 
/// unstructured data blob where the relationship cannot be enforced by
/// the database.
/// </summary>
public class GetEntityDependencySummaryByRelatedEntityIdRangeQuery : IQuery<IReadOnlyCollection<EntityDependencySummary>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetEntityDependencySummaryByRelatedEntityIdRangeQuery"/> class.
    /// </summary>
    public GetEntityDependencySummaryByRelatedEntityIdRangeQuery()
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
    /// Collection of entity ids to check for required dependencies. Each entity 
    /// must be of the type specified by <paramref name="entityDefinitionCode"/>.
    /// </param>
    public GetEntityDependencySummaryByRelatedEntityIdRangeQuery(string entityDefinitionCode, IEnumerable<int> entityIds)
    {
        EntityDefinitionCode = entityDefinitionCode;
        EntityIds = entityIds.ToArray();
    }

    /// <summary>
    /// <see cref="IEntityDefinition.EntityDefinitionCode"/> of the entity type
    /// to check.
    /// </summary>
    [Required]
    public string EntityDefinitionCode { get; set; } = string.Empty;

    /// <summary>
    /// Collection of entity ids to check for required dependencies. Each entity 
    /// must be of the type specified by <see cref="EntityDefinitionCode"/>.
    /// </summary>
    public IReadOnlyCollection<int> EntityIds { get; set; } = Array.Empty<int>();

    /// <summary>
    /// If set to true, then deletable dependencies will be excluded
    /// from the results, leaving only those relations that are required
    /// and cannot be removed i.e. those that would prevent the entity
    /// from being deleted.
    /// </summary>
    public bool ExcludeDeletable { get; set; }
}
