using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class GetEntityDependencySummaryByRelatedEntityHandler
        : IQueryHandler<GetEntityDependencySummaryByRelatedEntityQuery, ICollection<EntityDependencySummary>>
        , IPermissionRestrictedQueryHandler<GetEntityDependencySummaryByRelatedEntityQuery, ICollection<EntityDependencySummary>>
    {
        private readonly CofoundryDbContext _dbContext;
        private IQueryExecutor _queryExecutor;
        private readonly IEntityDefinitionRepository _entityDefinitionRepository;
        private readonly IPermissionRepository _permissionRepository;

        public GetEntityDependencySummaryByRelatedEntityHandler(
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

        public async Task<ICollection<EntityDependencySummary>> ExecuteAsync(GetEntityDependencySummaryByRelatedEntityQuery query, IExecutionContext executionContext)
        {
            // Where there are duplicates, prioritise relationships that cannot be deleted
            var dbDependencyPreResult = await _dbContext
                .UnstructuredDataDependencies
                .AsNoTracking()
                .Where(r => r.RelatedEntityDefinitionCode == query.EntityDefinitionCode && r.RelatedEntityId == query.EntityId)
                .ToListAsync();

            // Query is split because EF core cannot translate groupings yet
            var dbDependencyGroups = dbDependencyPreResult
                .GroupBy(r => new { r.RootEntityDefinitionCode, r.RootEntityId }, (d, e) => e.OrderByDescending(x => x.RelatedEntityCascadeActionId == (int)RelatedEntityCascadeAction.None).FirstOrDefault())
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

                    var entityDependencySummary = new EntityDependencySummary();
                    entityDependencySummary.Entity = entityMicroSummary.Value;

                    // relations for previous versions can be removed even when they are required.
                    entityDependencySummary.CanDelete =
                        dbDependency.RelatedEntityCascadeActionId != (int)RelatedEntityCascadeAction.None
                        || entityDependencySummary.Entity.IsPreviousVersion;

                    allRelatedEntities.Add(entityDependencySummary);
                }
            }

            return allRelatedEntities;
        }

        public IEnumerable<IPermissionApplication> GetPermissions(GetEntityDependencySummaryByRelatedEntityQuery query)
        {
            var entityDefinition = _entityDefinitionRepository.GetRequiredByCode(query.EntityDefinitionCode);
            if (entityDefinition == null) yield break;

            // Try and get a read permission for the entity.
            var permission = _permissionRepository.GetByEntityAndPermissionType(entityDefinition, CommonPermissionTypes.Read("Entity"));

            if (permission != null)
            {
                yield return permission;
            }
        }
    }
}
