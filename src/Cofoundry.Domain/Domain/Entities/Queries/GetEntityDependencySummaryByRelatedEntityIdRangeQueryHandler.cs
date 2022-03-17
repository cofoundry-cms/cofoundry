using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Returns information about entities that have a dependency on the entity 
/// being queried, typically becuase they reference the entity in an 
/// unstructured data blob where the relationship cannot be enforced by
/// the database.
/// </summary>
public class GetEntityDependencySummaryByRelatedEntityIdRangeQueryHandler
    : IQueryHandler<GetEntityDependencySummaryByRelatedEntityIdRangeQuery, ICollection<EntityDependencySummary>>
    , IPermissionRestrictedQueryHandler<GetEntityDependencySummaryByRelatedEntityIdRangeQuery, ICollection<EntityDependencySummary>>
{
    private readonly CofoundryDbContext _dbContext;
    private IQueryExecutor _queryExecutor;
    private readonly IEntityDefinitionRepository _entityDefinitionRepository;
    private readonly IPermissionRepository _permissionRepository;

    public GetEntityDependencySummaryByRelatedEntityIdRangeQueryHandler(
        CofoundryDbContext dbContext,
        IQueryExecutor queryExecutor,
        IEntityDefinitionRepository entityDefinitionRepository,
        IPermissionRepository permissionRepository
        )
    {
        _dbContext = dbContext;
        _queryExecutor = queryExecutor;
        _entityDefinitionRepository = entityDefinitionRepository;
        _permissionRepository = permissionRepository;
    }

    public async Task<ICollection<EntityDependencySummary>> ExecuteAsync(GetEntityDependencySummaryByRelatedEntityIdRangeQuery query, IExecutionContext executionContext)
    {
        var dbQuery = _dbContext
            .UnstructuredDataDependencies
            .AsNoTracking()
            .FilterByRelatedEntity(query.EntityDefinitionCode, query.EntityIds);

        if (query.ExcludeDeletable)
        {
            dbQuery = dbQuery.Where(r => r.RelatedEntityCascadeActionId == (int)RelatedEntityCascadeAction.None);
        }

        // Groupby still not suppored in EF 3.1 ¯\_(ツ)_/¯
        var queryResult = await dbQuery.ToListAsync();
        var dbDependencyGroups = queryResult
            .GroupBy(r => r.RootEntityDefinitionCode)
            .ToList();

        var allRelatedEntities = new List<EntityDependencySummary>();

        foreach (var dbDependencyGroup in dbDependencyGroups)
        {
            var definition = _entityDefinitionRepository.GetRequiredByCode(dbDependencyGroup.Key) as IDependableEntityDefinition;
            EntityNotFoundException.ThrowIfNull(definition, dbDependencyGroup.Key);

            var getEntitiesQuery = definition.CreateGetEntityMicroSummariesByIdRangeQuery(dbDependencyGroup.Select(e => e.RootEntityId));
            var entityMicroSummaries = await _queryExecutor.ExecuteAsync(getEntitiesQuery, executionContext);

            foreach (var entityMicroSummary in entityMicroSummaries.OrderBy(e => e.Value.RootEntityTitle))
            {
                var dbDependency = dbDependencyGroup.SingleOrDefault(e => e.RootEntityId == entityMicroSummary.Key);

                // relations for previous versions can be removed even when they are required.
                var canDelete = dbDependency.RelatedEntityCascadeActionId != (int)RelatedEntityCascadeAction.None
                    || entityMicroSummary.Value.IsPreviousVersion;

                if (query.ExcludeDeletable && canDelete)
                {
                    continue;
                }

                allRelatedEntities.Add(new EntityDependencySummary()
                {
                    Entity = entityMicroSummary.Value,
                    CanDelete = canDelete
                });
            }
        }

        // filter out duplicates, selecting the more restrictive entity first 
        var results = allRelatedEntities
            .GroupBy(e =>
                new { e.Entity.EntityDefinitionCode, e.Entity.RootEntityId },
                (k, v) => v.OrderBy(e => e.CanDelete).First())
            .ToList();

        return results;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetEntityDependencySummaryByRelatedEntityIdRangeQuery query)
    {
        var entityDefinition = _entityDefinitionRepository.GetRequiredByCode(query.EntityDefinitionCode);
        if (entityDefinition == null) yield break;

        var permission = _permissionRepository.GetByEntityAndPermissionType(entityDefinition, CommonPermissionTypes.Read("Entity"));
        if (permission != null) yield return permission;
    }
}
