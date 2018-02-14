using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple facade over custom entity data access queries/commands to them more discoverable
    /// in implementations.
    /// </summary>
    public interface ICustomEntityRepository
    {
        #region queries

        #region definitions

        Task<ICollection<CustomEntityDefinitionMicroSummary>> GetAllCustomEntityDefinitionMicroSummariesAsync(IExecutionContext executionContext = null);

        Task<CustomEntityDefinitionMicroSummary> GetCustomEntityDefinitionMicroSummaryByCodeAsync(string customEntityDefinitionCode, IExecutionContext executionContext = null);

        #endregion

        #region routes

        Task<CustomEntityRoute> GetCustomEntityRouteByPathAsync(GetCustomEntityRouteByPathQuery query, IExecutionContext executionContext = null);

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
        Task<IEnumerable<CustomEntityRoute>> GetCustomEntityRoutesByDefinitionCodeAsync(string customEntityDefinitionCode, IExecutionContext executionContext = null);

        Task<ICustomEntityRoutingRule> GetCustomEntityRoutingRuleByRouteFormatAsync(string routeFormat, IExecutionContext executionContext = null);

        #endregion

        #region CustomEntityRenderDetails

        Task<CustomEntityRenderDetails> GetCustomEntityRenderDetailsByIdAsync(GetCustomEntityRenderDetailsByIdQuery query, IExecutionContext executionContext = null);

        #endregion

        Task<ICollection<ICustomEntityRoutingRule>> GetAllCustomEntityRoutingRulesAsync(IExecutionContext executionContext = null);

        Task<CustomEntityDataModelSchema> GetCustomEntityDataModelSchemaDetailsByCodeAsync(string customEntityDefinitionCode, IExecutionContext executionContext = null);

        Task<CustomEntityDetails> GetCustomEntityDetailsByIdAsync(int id, IExecutionContext executionContext = null);

        Task<IEnumerable<CustomEntityRenderSummary>> GetCustomEntityRenderSummariesByDefinitionCodeAsync(GetCustomEntityRenderSummariesByDefinitionCodeQuery query, IExecutionContext executionContext = null);

        Task<CustomEntityRenderSummary> GetCustomEntityRenderSummaryByIdAsync(GetCustomEntityRenderSummaryByIdQuery query, IExecutionContext executionContext = null);

        Task<IDictionary<int, CustomEntityRenderSummary>> GetCustomEntityRenderSummariesByIdRangeAsync(GetCustomEntityRenderSummariesByIdRangeQuery query, IExecutionContext executionContext = null);

        Task<IDictionary<int, CustomEntitySummary>> GetCustomEntitySummariesByIdRangeAsync(IEnumerable<int> ids, IExecutionContext executionContext = null);

        Task<CustomEntityVersionPageBlockRenderDetails> GetCustomEntityVersionPageBlockRenderDetailsByIdAsync(GetCustomEntityVersionPageBlockRenderDetailsByIdQuery query, IExecutionContext executionContext = null);

        Task<IEnumerable<CustomEntityVersionSummary>> GetCustomEntityVersionSummariesByCustomEntityIdAsync(int id, IExecutionContext executionContext = null);

        Task<bool> IsCustomEntityPathUniqueAsync(IsCustomEntityPathUniqueQuery query, IExecutionContext executionContext = null);

        Task<PagedQueryResult<CustomEntitySummary>> SearchCustomEntitySummariesAsync(SearchCustomEntitySummariesQuery query, IExecutionContext executionContext = null);

        Task<PagedQueryResult<CustomEntityRenderSummary>> SearchCustomEntityRenderSummariesAsync(SearchCustomEntityRenderSummariesQuery query, IExecutionContext executionContext = null);

        #endregion

        #region commands

        Task<int> AddCustomEntityAsync(AddCustomEntityCommand command, IExecutionContext executionContext = null);

        /// <summary>
        /// Creates a new draft version of a custom entity from the currently published version. If there
        /// isn't a currently published version then an exception will be thrown. An exception is also 
        /// thrown if there is already a draft version.
        /// </summary>
        Task<int> AddCustomEntityDraftVersionAsync(AddCustomEntityDraftVersionCommand command, IExecutionContext executionContext = null);

        Task<int> AddCustomEntityVersionPageBlockAsync(AddCustomEntityVersionPageBlockCommand command, IExecutionContext executionContext = null);

        Task DeleteCustomEntityAsync(int customEntityId, IExecutionContext executionContext = null);

        Task DeleteCustomEntityDraftVersionAsync(int customEntityId, IExecutionContext executionContext = null);

        Task DeleteCustomEntityVersionPageBlockAsync(int customEntityVersionPageBlockId, IExecutionContext executionContext = null);

        Task EnsureCustomEntityDefinitionExistsAsync(string customEntityDefinitionCode, IExecutionContext executionContext = null);

        Task MoveCustomEntityVersionPageBlockAsync(MoveCustomEntityVersionPageBlockCommand command, IExecutionContext executionContext = null);

        Task PublishCustomEntityAsync(PublishCustomEntityCommand command, IExecutionContext executionContext = null);

        Task ReOrderCustomEntitiesAsync(ReOrderCustomEntitiesCommand command, IExecutionContext executionContext = null);

        Task UnPublishCustomEntityAsync(int customEntityId, IExecutionContext executionContext = null);

        Task UpdateCustomEntityDraftVersionAsync(UpdateCustomEntityDraftVersionCommand command, IExecutionContext executionContext = null);

        Task UpdateCustomEntityOrderingPositionAsync(UpdateCustomEntityOrderingPositionCommand command, IExecutionContext executionContext = null);

        Task UpdateCustomEntityUrlAsync(UpdateCustomEntityUrlCommand command, IExecutionContext executionContext = null);

        Task UpdateCustomEntityVersionPageBlockAsync(UpdateCustomEntityVersionPageBlockCommand command, IExecutionContext executionContext = null);

        #endregion
    }
}
