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
    [AdminApiRoute("custom-entities")]
    public class CustomEntitiesApiController : BaseAdminApiController
    {
        #region private member variables

        private const string ID_ROUTE = "{customEntityId:int}";

        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;

        #endregion

        #region constructor

        public CustomEntitiesApiController(
            IQueryExecutor queryExecutor,
            IApiResponseHelper apiResponseHelper
            )
        {
            _queryExecutor = queryExecutor;
            _apiResponseHelper = apiResponseHelper;
        }

        #endregion

        #region routes

        #region queries

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] SearchCustomEntitySummariesQuery query, [FromQuery] GetByIdRangeQuery<CustomEntitySummary> rangeQuery)
        {
            if (rangeQuery != null && rangeQuery.Ids != null)
            {
                var rangeResults = await _queryExecutor.ExecuteAsync(rangeQuery);
                return _apiResponseHelper.SimpleQueryResponse(this, rangeResults.FilterAndOrderByKeys(rangeQuery.Ids));
            }

            if (query == null) query = new SearchCustomEntitySummariesQuery();

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        [HttpGet(ID_ROUTE)]
        public async Task<IActionResult> Get(int customEntityId)
        {
            var result = await _queryExecutor.GetByIdAsync<CustomEntityDetails>(customEntityId);
            return _apiResponseHelper.SimpleQueryResponse(this, result);
        }

        #endregion

        #region commands

        [HttpPost]
        public async Task<IActionResult> Post([ModelBinder(BinderType = typeof(CustomEntityDataModelCommandModelBinder))] AddCustomEntityCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPut("ordering")]
        public async Task<IActionResult> PutOrdering([FromBody] ReOrderCustomEntitiesCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPut(ID_ROUTE + "/url")]
        public async Task<IActionResult> PutCustomEntityUrl(int customEntityId, [FromBody] UpdateCustomEntityUrlCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpDelete(ID_ROUTE)]
        public async Task<IActionResult> Delete(int customEntityId)
        {
            var command = new DeleteCustomEntityCommand();
            command.CustomEntityId = customEntityId;

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        #endregion

        #endregion
    }
}