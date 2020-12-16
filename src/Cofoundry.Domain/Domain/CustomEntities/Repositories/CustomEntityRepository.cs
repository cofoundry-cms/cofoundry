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
    [Obsolete("Use the new IContentRepository instead.")]
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

        public Task<ICollection<CustomEntityDefinitionMicroSummary>> GetAllCustomEntityDefinitionMicroSummariesAsync(IExecutionContext executionContext = null)
        {
            var query = new GetAllCustomEntityDefinitionMicroSummariesQuery();
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        public Task<CustomEntityDefinitionMicroSummary> GetCustomEntityDefinitionMicroSummaryByCodeAsync(string customEntityDefinitionCode, IExecutionContext executionContext = null)
        {
            var query = new GetCustomEntityDefinitionMicroSummaryByCodeQuery(customEntityDefinitionCode);
            return _queryExecutor.ExecuteAsync(query, executionContext);
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
        public Task<ICollection<CustomEntityRoute>> GetCustomEntityRoutesByDefinitionCodeAsync(string customEntityDefinitionCode, IExecutionContext executionContext = null)
        {
            var query = new GetCustomEntityRoutesByDefinitionCodeQuery(customEntityDefinitionCode);
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        public Task<ICustomEntityRoutingRule> GetCustomEntityRoutingRuleByRouteFormatAsync(string routeFormat, IExecutionContext executionContext = null)
        {
            var query = new GetCustomEntityRoutingRuleByRouteFormatQuery(routeFormat);
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        public Task<ICollection<ICustomEntityRoutingRule>> GetAllCustomEntityRoutingRulesAsync(IExecutionContext executionContext = null)
        {
            var query = new GetAllCustomEntityRoutingRulesQuery();
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        #endregion

        #region CustomEntityRenderDetails

        public Task<CustomEntityRenderDetails> GetCustomEntityRenderDetailsByIdAsync(GetCustomEntityRenderDetailsByIdQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        #endregion

        #region CustomEntityRenderSummary

        public Task<PagedQueryResult<CustomEntityRenderSummary>> SearchCustomEntityRenderSummariesAsync(SearchCustomEntityRenderSummariesQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        public Task<ICollection<CustomEntityRenderSummary>> GetCustomEntityRenderSummariesByDefinitionCodeAsync(GetCustomEntityRenderSummariesByDefinitionCodeQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        public Task<CustomEntityRenderSummary> GetCustomEntityRenderSummaryByIdAsync(GetCustomEntityRenderSummaryByIdQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        public Task<IDictionary<int, CustomEntityRenderSummary>> GetCustomEntityRenderSummariesByIdRangeAsync(GetCustomEntityRenderSummariesByIdRangeQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        /// <summary>
        /// Returns custom entities filtered on the url slug value. This query
        /// can return multiple custom entities because unique UrlSlugs is an
        /// optional setting on the custom entity definition.
        /// </summary>
        /// <param name="query">Query parameters.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task<ICollection<CustomEntityRenderSummary>> GetCustomEntityRenderSummariesByUrlSlugAsync(GetCustomEntityRenderSummariesByUrlSlugQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        #endregion

        #region CustomEntitySummary

        /// <summary>
        /// A workflow non-specifc search of custom entities which returns basic
        /// custom entity information with workflow status and model data for the
        /// latest version. Designed to be used in the admin panel and not in a 
        /// version-sensitive context sach as a public webpage.
        /// </summary>
        /// <param name="query">Query parameters.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task<PagedQueryResult<CustomEntitySummary>> SearchCustomEntitySummariesAsync(SearchCustomEntitySummariesQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        /// <summary>
        /// An id range query for custom entities which returns basic
        /// custom entity information with workflow status and model data for the
        /// latest version. The query is not version-sensitive and is designed to be 
        /// used in the admin panel and not in a version-sensitive context such as a 
        /// public webpage.
        /// </summary>
        /// <param name="customEntityIds">
        /// Collection of custom entity ids to find. Any ids unable to be
        /// located will not be present in the result.
        /// </param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task<IDictionary<int, CustomEntitySummary>> GetCustomEntitySummariesByIdRangeAsync(IEnumerable<int> customEntityIds, IExecutionContext executionContext = null)
        {
            var query = new GetCustomEntitySummariesByIdRangeQuery(customEntityIds);
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        #endregion

        public Task<CustomEntityDataModelSchema> GetCustomEntityDataModelSchemaDetailsByCodeAsync(string customEntityDefinitionCode, IExecutionContext executionContext = null)
        {
            var query = new GetCustomEntityDataModelSchemaDetailsByDefinitionCodeQuery(customEntityDefinitionCode);
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        /// <summary>
        /// Returns detailed information on a custom entity and it's latest version. This 
        /// query is primarily used in the admin area because it is not version-specific
        /// and the CustomEntityDetails projection includes audit data and other additional 
        /// information that should normally be hidden from a customer facing app.
        /// </summary>
        /// <param name="customEntityId">Id of the custom entity to find.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task<CustomEntityDetails> GetCustomEntityDetailsByIdAsync(int customEntityId, IExecutionContext executionContext = null)
        {
            var query = new GetCustomEntityDetailsByIdQuery(customEntityId);
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        /// <summary>
        /// Returns data for a specific custom entity page block by it's id. Because
        /// the mapped display model may contain other versioned entities, you can 
        /// optionally pass down a PublishStatusQuery to use in the mapping process.
        /// </summary>
        /// <param name="query">Query parameters.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task<CustomEntityVersionPageBlockRenderDetails> GetCustomEntityVersionPageBlockRenderDetailsByIdAsync(GetCustomEntityVersionPageBlockRenderDetailsByIdQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        public Task<PagedQueryResult<CustomEntityVersionSummary>> GetCustomEntityVersionSummariesByCustomEntityIdAsync(GetCustomEntityVersionSummariesByCustomEntityIdQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        public Task<bool> IsCustomEntityPathUniqueAsync(IsCustomEntityUrlSlugUniqueQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query);
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

        public async Task<int> AddCustomEntityVersionPageBlockAsync(AddCustomEntityVersionPageBlockCommand command, IExecutionContext executionContext = null)
        {
            await _commandExecutor.ExecuteAsync(command, executionContext);

            return command.OutputCustomEntityVersionPageBlockId;
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

        public Task DeleteCustomEntityVersionPageBlockAsync(int customEntityVersionPageBlockId, IExecutionContext executionContext = null)
        {
            var command = new DeleteCustomEntityVersionPageBlockCommand() { CustomEntityVersionPageBlockId = customEntityVersionPageBlockId };
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        public Task EnsureCustomEntityDefinitionExistsAsync(string customEntityDefinitionCode, IExecutionContext executionContext = null)
        {
            var command = new EnsureCustomEntityDefinitionExistsCommand() { CustomEntityDefinitionCode = customEntityDefinitionCode };
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        public Task MoveCustomEntityVersionPageBlockAsync(MoveCustomEntityVersionPageBlockCommand command, IExecutionContext executionContext = null)
        {
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        public Task PublishCustomEntityAsync(PublishCustomEntityCommand command, IExecutionContext executionContext = null)
        {
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

        public Task UpdateCustomEntityVersionPageBlockAsync(UpdateCustomEntityVersionPageBlockCommand command, IExecutionContext executionContext = null)
        {
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        #endregion
    }
}
