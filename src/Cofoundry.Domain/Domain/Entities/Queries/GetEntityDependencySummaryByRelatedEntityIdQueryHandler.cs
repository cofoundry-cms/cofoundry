using Cofoundry.Domain.CQS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Returns information about entities that have a dependency on the entity 
    /// being queried, typically becuase they reference the entity in an 
    /// unstructured data blob where the relationship cannot be enforced by
    /// the database.
    /// </summary>
    public class GetEntityDependencySummaryByRelatedEntityIdQueryHandler
        : IQueryHandler<GetEntityDependencySummaryByRelatedEntityIdQuery, ICollection<EntityDependencySummary>>
        , IIgnorePermissionCheckHandler
    {
        private IQueryExecutor _queryExecutor;

        public GetEntityDependencySummaryByRelatedEntityIdQueryHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        public async Task<ICollection<EntityDependencySummary>> ExecuteAsync(GetEntityDependencySummaryByRelatedEntityIdQuery query, IExecutionContext executionContext)
        {
            // Query and permissions are delegated to the GetByIdRange version
            var delegateQuery = new GetEntityDependencySummaryByRelatedEntityIdRangeQuery(query.EntityDefinitionCode, new int[] { query.EntityId })
            {
                ExcludeDeletable = query.ExcludeDeletable
            };

            var results = await _queryExecutor.ExecuteAsync(delegateQuery, executionContext);
            return results;
        }
    }
}
