using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin
{
    public class CustomEntitiesApiController : BaseAdminApiController
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;

        public CustomEntitiesApiController(
            IQueryExecutor queryExecutor,
            IApiResponseHelper apiResponseHelper
            )
        {
            _queryExecutor = queryExecutor;
            _apiResponseHelper = apiResponseHelper;
        }

        #region queries

        public async Task<JsonResult> Get([FromQuery] SearchCustomEntitySummariesQuery query, [FromQuery] GetCustomEntitySummariesByIdRangeQuery rangeQuery)
        {
            if (rangeQuery != null && rangeQuery.CustomEntityIds != null)
            {
                var rangeResults = await _queryExecutor.ExecuteAsync(rangeQuery);
                return _apiResponseHelper.SimpleQueryResponse(rangeResults.FilterAndOrderByKeys(rangeQuery.CustomEntityIds));
            }

            if (query == null) query = new SearchCustomEntitySummariesQuery();
            ApiPagingHelper.SetDefaultBounds(query);

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(results);
        }

        public async Task<JsonResult> GetById(int customEntityId)
        {
            var query = new GetCustomEntityDetailsByIdQuery(customEntityId);
            var result = await _queryExecutor.ExecuteAsync(query);

            return _apiResponseHelper.SimpleQueryResponse(result);
        }

        #endregion

        #region commands

        public Task<JsonResult> Post([ModelBinder(BinderType = typeof(CustomEntityDataModelCommandModelBinder))] AddCustomEntityCommand command)
        {
            return _apiResponseHelper.RunCommandAsync(command);
        }

        public Task<JsonResult> PutOrdering([FromBody] ReOrderCustomEntitiesCommand command)
        {
            return _apiResponseHelper.RunCommandAsync(command);
        }

        public Task<JsonResult> PutCustomEntityUrl(int customEntityId, [FromBody] UpdateCustomEntityUrlCommand command)
        {
            return _apiResponseHelper.RunCommandAsync(command);
        }

        public Task<JsonResult> Delete(int customEntityId)
        {
            var command = new DeleteCustomEntityCommand();
            command.CustomEntityId = customEntityId;

            return _apiResponseHelper.RunCommandAsync(command);
        }

        public Task<JsonResult> PostDuplicate([FromBody] DuplicateCustomEntityCommand command)
        {
            return _apiResponseHelper.RunCommandAsync(command);
        }

        #endregion
    }
}