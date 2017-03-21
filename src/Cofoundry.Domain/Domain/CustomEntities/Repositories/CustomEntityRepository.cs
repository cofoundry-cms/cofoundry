using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple facade over custom entity data access queries/commands to them more discoverable
    /// in implementations.
    /// </summary>
    public class CustomEntityRepository : ICustomEntityRepository
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;

        public CustomEntityRepository(
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor
            )
        {
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
        }

        #endregion

        #region queries

        #region definitions

        public IEnumerable<CustomEntityDefinitionMicroSummary> GetAllCustomEntityDefinitionMicroSummaries(IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetAll<CustomEntityDefinitionMicroSummary>(executionContext);
        }

        public Task<IEnumerable<CustomEntityDefinitionMicroSummary>> GetAllCustomEntityDefinitionMicroSummariesAsync(IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetAllAsync<CustomEntityDefinitionMicroSummary>(executionContext);
        }

        public CustomEntityDefinitionMicroSummary GetCustomEntityDefinitionMicroSummaryById(string customEntityDefinitionCode, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetById<CustomEntityDefinitionMicroSummary>(customEntityDefinitionCode, executionContext);
        }

        public Task<CustomEntityDefinitionMicroSummary> GetCustomEntityDefinitionMicroSummaryByIdAsync(string customEntityDefinitionCode, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetByIdAsync<CustomEntityDefinitionMicroSummary>(customEntityDefinitionCode, executionContext);
        }

        #endregion

        #region routes

        public Task<CustomEntityRoute> GetCustomEntityRouteByPathAsync(GetCustomEntityRouteByPathQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        /// <summary>
        /// Gets CustomEntityRoute data for all custom entities of a 
        /// specific type. These route objects are small and cached which
        /// makes them good for quick lookups.
        /// </summary>
        /// <param name="customEntityDefinitionCode">
        /// The code identifier for the custom entity type
        /// to query for.
        /// </param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public IEnumerable<CustomEntityRoute> GetCustomEntityRoutesByDefinitionCode(string customEntityDefinitionCode, IExecutionContext executionContext = null)
        {
            var query = new GetCustomEntityRoutesByDefinitionCodeQuery(customEntityDefinitionCode);
            return _queryExecutor.Execute(query, executionContext);
        }

        /// <summary>
        /// Gets CustomEntityRoute data for all custom entities of a 
        /// specific type. These route objects are small and cached which
        /// makes them good for quick lookups.
        /// </summary>
        /// <param name="customEntityDefinitionCode">
        /// The code identifier for the custom entity type
        /// to query for.
        /// </param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task<IEnumerable<CustomEntityRoute>> GetCustomEntityRoutesByDefinitionCodeAsync(string customEntityDefinitionCode, IExecutionContext executionContext = null)
        {
            var query = new GetCustomEntityRoutesByDefinitionCodeQuery(customEntityDefinitionCode);
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        public ICustomEntityRoutingRule GetCustomEntityRoutingRuleByRouteFormat(string routeFormat, IExecutionContext executionContext = null)
        {
            var query = new GetCustomEntityRoutingRuleByRouteFormatQuery(routeFormat);
            return _queryExecutor.Execute(query, executionContext);
        }

        public Task<ICustomEntityRoutingRule> GetCustomEntityRoutingRuleByRouteFormatAsync(string routeFormat, IExecutionContext executionContext = null)
        {
            var query = new GetCustomEntityRoutingRuleByRouteFormatQuery(routeFormat);
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        #endregion

        #region CustomEntityRenderDetails

        public CustomEntityRenderDetails GetCustomEntityRenderDetailsById(GetCustomEntityRenderDetailsByIdQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.Execute(query, executionContext);
        }

        public Task<CustomEntityRenderDetails> GetCustomEntityRenderDetailsByIdAsync(GetCustomEntityRenderDetailsByIdQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        #endregion

        public IEnumerable<ICustomEntityRoutingRule> GetAllCustomEntityRoutingRules(IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetAll<ICustomEntityRoutingRule>(executionContext);
        }
        
        public Task<IEnumerable<ICustomEntityRoutingRule>> GetAllCustomEntityRoutingRulesAsync(IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetAllAsync<ICustomEntityRoutingRule>(executionContext);
        }

        public Task<CustomEntityDataModelSchema> GetCustomEntityDataModelSchemaDetailsByCodeAsync(string customEntityDefinitionCode, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetByIdAsync<CustomEntityDataModelSchema>(customEntityDefinitionCode, executionContext);
        }

        public Task<CustomEntityDetails> GetCustomEntityDetailsByIdAsync(int id, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetByIdAsync<CustomEntityDetails>(id, executionContext);
        }
        
        public IEnumerable<CustomEntityRenderSummary> GetCustomEntityRenderSummariesByDefinitionCode(GetCustomEntityRenderSummariesByDefinitionCodeQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.Execute(query, executionContext);
        }

        public Task<IEnumerable<CustomEntityRenderSummary>> GetCustomEntityRenderSummariesByDefinitionCodeAsync(GetCustomEntityRenderSummariesByDefinitionCodeQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        public CustomEntityRenderSummary GetCustomEntityRenderSummaryById(GetCustomEntityRenderSummaryByIdQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.Execute(query, executionContext);
        }

        public Task<CustomEntityRenderSummary> GetCustomEntityRenderSummaryByIdAsync(GetCustomEntityRenderSummaryByIdQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        public IDictionary<int, CustomEntityRenderSummary> GetCustomEntityRenderSummariesByIdRange(GetCustomEntityRenderSummariesByIdRangeQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.Execute(query, executionContext);
        }

        public Task<Dictionary<int, CustomEntityRenderSummary>> GetCustomEntityRenderSummariesByIdRangeAsync(GetCustomEntityRenderSummariesByIdRangeQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }
        
        public IDictionary<int, CustomEntitySummary> GetCustomEntityRenderSummaryByIdRange(IEnumerable<int> ids, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetByIdRange<CustomEntitySummary>(ids, executionContext);
        }

        public Task<IDictionary<int, CustomEntitySummary>> GetCustomEntityRenderSummaryByIdRangeAsync(IEnumerable<int> ids, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetByIdRangeAsync<CustomEntitySummary>(ids, executionContext);
        }

        public Task<CustomEntityVersionPageModuleRenderDetails> GetCustomEntityVersionPageModuleRenderDetailsByIdAsync(GetCustomEntityVersionPageModuleRenderDetailsByIdQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        public Task<IEnumerable<CustomEntityVersionSummary>> GetCustomEntityVersionSummariesByCustomEntityIdAsync(int id, IExecutionContext executionContext = null)
        {
            var query = new GetCustomEntityVersionSummariesByCustomEntityIdQuery() { CustomEntityId = id };
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        public bool IsCustomEntityPathUnique(IsCustomEntityPathUniqueQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.Execute(query);
        }

        public Task<bool> IsCustomEntityPathUniqueAsync(IsCustomEntityPathUniqueQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query);
        }

        public Task<PagedQueryResult<CustomEntitySummary>> SearchCustomEntitySummariesAsync(SearchCustomEntitySummariesQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        public PagedQueryResult<CustomEntityRenderSummary> SearchCustomEntityRenderSummaries(SearchCustomEntityRenderSummariesQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.Execute(query, executionContext);
        }

        public Task<PagedQueryResult<CustomEntityRenderSummary>> SearchCustomEntityRenderSummariesAsync(SearchCustomEntityRenderSummariesQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        #endregion

        #region commands

        public async Task<int> AddCustomEntityAsync(AddCustomEntityCommand command, IExecutionContext executionContext = null)
        {
            await _commandExecutor.ExecuteAsync(command, executionContext);

            return command.OutputCustomEntityId;
        }

        /// <summary>
        /// Creates a new draft version of a custom entity from the currently published version. If there
        /// isn't a currently published version then an exception will be thrown. An exception is also 
        /// thrown if there is already a draft version.
        /// </summary>
        public async Task<int> AddCustomEntityDraftVersionAsync(AddCustomEntityDraftVersionCommand command, IExecutionContext executionContext = null)
        {
            await _commandExecutor.ExecuteAsync(command, executionContext);

            return command.OutputCustomEntityVersionId;
        }

        public async Task<int> AddCustomEntityVersionPageModuleAsync(AddCustomEntityVersionPageModuleCommand command, IExecutionContext executionContext = null)
        {
            await _commandExecutor.ExecuteAsync(command, executionContext);

            return command.OutputCustomEntityVersionId;
        }

        public Task DeleteCustomEntityAsync(int customEntityId, IExecutionContext executionContext = null)
        {
            var command = new DeleteCustomEntityCommand() { CustomEntityId = customEntityId };
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        public Task DeleteCustomEntityDraftVersionAsync(int customEntityId, IExecutionContext executionContext = null)
        {
            var command = new DeleteCustomEntityDraftVersionCommand() { CustomEntityId = customEntityId };
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        public Task DeleteCustomEntityVersionPageModuleAsync(int customEntityVersionPageModuleId, IExecutionContext executionContext = null)
        {
            var command = new DeleteCustomEntityVersionPageModuleCommand() { CustomEntityVersionPageModuleId = customEntityVersionPageModuleId };
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        public Task EnsureCustomEntityDefinitionExistsAsync(string customEntityDefinitionCode, IExecutionContext executionContext = null)
        {
            var command = new EnsureCustomEntityDefinitionExistsCommand() { CustomEntityDefinitionCode = customEntityDefinitionCode };
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        public Task MoveCustomEntityVersionPageModuleAsync(MoveCustomEntityVersionPageModuleCommand command, IExecutionContext executionContext = null)
        {
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        public Task PublishCustomEntityAsync(int customEntityId, IExecutionContext executionContext = null)
        {
            var command = new PublishCustomEntityCommand() { CustomEntityId = customEntityId };
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        public Task ReOrderCustomEntitiesAsync(ReOrderCustomEntitiesCommand command, IExecutionContext executionContext = null)
        {
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        public Task UnPublishCustomEntityAsync(int customEntityId, IExecutionContext executionContext = null)
        {
            var command = new UnPublishCustomEntityCommand() { CustomEntityId = customEntityId };
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        public Task UpdateCustomEntityDraftVersionAsync(UpdateCustomEntityDraftVersionCommand command, IExecutionContext executionContext = null)
        {
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        public Task UpdateCustomEntityOrderingPositionAsync(UpdateCustomEntityOrderingPositionCommand command, IExecutionContext executionContext = null)
        {
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        public Task UpdateCustomEntityUrlAsync(UpdateCustomEntityUrlCommand command, IExecutionContext executionContext = null)
        {
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        public Task UpdateCustomEntityVersionPageModuleAsync(UpdateCustomEntityVersionPageModuleCommand command, IExecutionContext executionContext = null)
        {
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        #endregion
    }
}
