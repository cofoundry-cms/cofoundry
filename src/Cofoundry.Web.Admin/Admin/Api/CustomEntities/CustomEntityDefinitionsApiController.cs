using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin
{
    public class CustomEntityDefinitionsApiController : BaseAdminApiController
    {
        private const string ID_ROUTE = "{customEntityDefinitionCode}";

        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;

        public CustomEntityDefinitionsApiController(
            IQueryExecutor queryExecutor,
            IApiResponseHelper apiResponseHelper
            )
        {
            _queryExecutor = queryExecutor;
            _apiResponseHelper = apiResponseHelper;
        }

        #region queries

        public async Task<IActionResult> Get()
        {
            var results = await _queryExecutor.ExecuteAsync(new GetAllCustomEntityDefinitionSummariesQuery());
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        public async Task<IActionResult> GetById(string customEntityDefinitionCode)
        {
            var result = await _queryExecutor.ExecuteAsync(new GetCustomEntityDefinitionSummaryByCodeQuery(customEntityDefinitionCode));
            return _apiResponseHelper.SimpleQueryResponse(this, result);
        }
        
        public async Task<IActionResult> GetCustomEntityRoutes(string customEntityDefinitionCode)
        {
            var query = new GetPageRoutesByCustomEntityDefinitionCodeQuery(customEntityDefinitionCode);
            var result = await _queryExecutor.ExecuteAsync(query);

            return _apiResponseHelper.SimpleQueryResponse(this, result);
        }

        public async Task<IActionResult> GetDataModelSchema(string customEntityDefinitionCode)
        {
            var result = await _queryExecutor.ExecuteAsync(new GetCustomEntityDataModelSchemaDetailsByDefinitionCodeQuery(customEntityDefinitionCode));
            return _apiResponseHelper.SimpleQueryResponse(this, result);
        }
        
        public async Task<IActionResult> GetCustomEntities(string customEntityDefinitionCode, [FromQuery] SearchCustomEntitySummariesQuery query)
        {
            if (query == null) query = new SearchCustomEntitySummariesQuery();
            query.CustomEntityDefinitionCode = customEntityDefinitionCode;
            ApiPagingHelper.SetDefaultBounds(query);

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        #endregion
    }
}