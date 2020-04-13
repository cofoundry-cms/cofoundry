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
    [Obsolete("Use the new IContentRepository instead.")]
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
        Task<ICollection<CustomEntityRoute>> GetCustomEntityRoutesByDefinitionCodeAsync(string customEntityDefinitionCode, IExecutionContext executionContext = null);

        Task<ICustomEntityRoutingRule> GetCustomEntityRoutingRuleByRouteFormatAsync(string routeFormat, IExecutionContext executionContext = null);

        #endregion

        #region CustomEntityRenderDetails

        Task<CustomEntityRenderDetails> GetCustomEntityRenderDetailsByIdAsync(GetCustomEntityRenderDetailsByIdQuery query, IExecutionContext executionContext = null);

        #endregion

        #region CustomEntityRenderSummary

        Task<PagedQueryResult<CustomEntityRenderSummary>> SearchCustomEntityRenderSummariesAsync(SearchCustomEntityRenderSummariesQuery query, IExecutionContext executionContext = null);

        Task<ICollection<CustomEntityRenderSummary>> GetCustomEntityRenderSummariesByDefinitionCodeAsync(GetCustomEntityRenderSummariesByDefinitionCodeQuery query, IExecutionContext executionContext = null);

        Task<CustomEntityRenderSummary> GetCustomEntityRenderSummaryByIdAsync(GetCustomEntityRenderSummaryByIdQuery query, IExecutionContext executionContext = null);

        Task<IDictionary<int, CustomEntityRenderSummary>> GetCustomEntityRenderSummariesByIdRangeAsync(GetCustomEntityRenderSummariesByIdRangeQuery query, IExecutionContext executionContext = null);

        /// <summary>
        /// Returns custom entities filtered on the url slug value. This query
        /// can return multiple custom entities because unique UrlSlugs is an
        /// optional setting on the custom entity definition.
        /// </summary>
        /// <param name="query">Query parameters.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<ICollection<CustomEntityRenderSummary>> GetCustomEntityRenderSummariesByUrlSlugAsync(GetCustomEntityRenderSummariesByUrlSlugQuery query, IExecutionContext executionContext = null);

        #endregion

        Task<ICollection<ICustomEntityRoutingRule>> GetAllCustomEntityRoutingRulesAsync(IExecutionContext executionContext = null);

        Task<CustomEntityDataModelSchema> GetCustomEntityDataModelSchemaDetailsByCodeAsync(string customEntityDefinitionCode, IExecutionContext executionContext = null);

        /// <summary>
        /// Returns detailed information on a custom entity and it's latest version. This 
        /// query is primarily used in the admin area because it is not version-specific
        /// and the CustomEntityDetails projection includes audit data and other additional 
        /// information that should normally be hidden from a customer facing app.
        /// </summary>
        /// <param name="customEntityId">Id of the custom entity to find.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<CustomEntityDetails> GetCustomEntityDetailsByIdAsync(int customEntityId, IExecutionContext executionContext = null);

        /// <summary>
        /// Returns data for a specific custom entity page block by it's id. Because
        /// the mapped display model may contain other versioned entities, you can 
        /// optionally pass down a PublishStatusQuery to use in the mapping process.
        /// </summary>
        /// <param name="query">Query parameters.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<CustomEntityVersionPageBlockRenderDetails> GetCustomEntityVersionPageBlockRenderDetailsByIdAsync(GetCustomEntityVersionPageBlockRenderDetailsByIdQuery query, IExecutionContext executionContext = null);

        Task<PagedQueryResult<CustomEntityVersionSummary>> GetCustomEntityVersionSummariesByCustomEntityIdAsync(GetCustomEntityVersionSummariesByCustomEntityIdQuery query, IExecutionContext executionContext = null);

        Task<bool> IsCustomEntityPathUniqueAsync(IsCustomEntityUrlSlugUniqueQuery query, IExecutionContext executionContext = null);

        Task<PagedQueryResult<CustomEntitySummary>> SearchCustomEntitySummariesAsync(SearchCustomEntitySummariesQuery query, IExecutionContext executionContext = null);

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
