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

        public async Task<IActionResult> Get([FromQuery] SearchCustomEntitySummariesQuery query, [FromQuery] GetCustomEntitySummariesByIdRangeQuery rangeQuery)
        {
            if (rangeQuery != null && rangeQuery.CustomEntityIds != null)
            {
                var rangeResults = await _queryExecutor.ExecuteAsync(rangeQuery);
                return _apiResponseHelper.SimpleQueryResponse(this, rangeResults.FilterAndOrderByKeys(rangeQuery.CustomEntityIds));
            }

            if (query == null) query = new SearchCustomEntitySummariesQuery();
            ApiPagingHelper.SetDefaultBounds(query);

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        public async Task<IActionResult> GetById(int customEntityId)
        {
            var query = new GetCustomEntityDetailsByIdQuery(customEntityId);
            var result = await _queryExecutor.ExecuteAsync(query);

            return _apiResponseHelper.SimpleQueryResponse(this, result);
        }

        #endregion

        #region commands

        [HttpPost]
        public async Task<IActionResult> Post([ModelBinder(BinderType = typeof(CustomEntityDataModelCommandModelBinder))] AddCustomEntityCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPut]
        public async Task<IActionResult> PutOrdering([FromBody] ReOrderCustomEntitiesCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPut]
        public async Task<IActionResult> PutCustomEntityUrl(int customEntityId, [FromBody] UpdateCustomEntityUrlCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int customEntityId)
        {
            var command = new DeleteCustomEntityCommand();
            command.CustomEntityId = customEntityId;

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPost]
        public async Task<IActionResult> PostDuplicate([FromBody] DuplicateCustomEntityCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        #endregion
    }
}