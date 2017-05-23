using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Web.WebApi;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin
{
    [AdminApiRoute("custom-entity-definitions")]
    public class CustomEntityDefinitionsApiController : BaseAdminApiController
    {
        #region private member variables

        private const string ID_ROUTE = "{customEntityDefinitionCode}";

        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;

        #endregion

        #region constructor

        public CustomEntityDefinitionsApiController(
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
        public async Task<IActionResult> Get()
        {
            var results = await _queryExecutor.GetAllAsync<CustomEntityDefinitionSummary>();
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        [HttpGet(ID_ROUTE)]
        public async Task<IActionResult> Get(string customEntityDefinitionCode)
        {
            var result = await _queryExecutor.GetByIdAsync<CustomEntityDefinitionSummary>(customEntityDefinitionCode);
            return _apiResponseHelper.SimpleQueryResponse(this, result);
        }
        
        [HttpGet(ID_ROUTE + "/routes")]
        public async Task<IActionResult> GetCustomEntityRoutes(string customEntityDefinitionCode)
        {
            var query = new GetPageRoutesByCustomEntityDefinitionCodeQuery(customEntityDefinitionCode);
            var result = await _queryExecutor.ExecuteAsync(query);

            return _apiResponseHelper.SimpleQueryResponse(this, result);
        }

        [HttpGet(ID_ROUTE + "/data-model-schema")]
        public async Task<IActionResult> GetDataModelSchema(string customEntityDefinitionCode)
        {
            var result = await _queryExecutor.GetByIdAsync<CustomEntityDataModelSchema>(customEntityDefinitionCode);
            return _apiResponseHelper.SimpleQueryResponse(this, result);
        }

        
        [HttpGet(ID_ROUTE + "/custom-entities")]
        public async Task<IActionResult> Get(string customEntityDefinitionCode, [FromQuery] SearchCustomEntitySummariesQuery query)
        {
            if (query == null) query = new SearchCustomEntitySummariesQuery();
            query.CustomEntityDefinitionCode = customEntityDefinitionCode;

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        #endregion

        #endregion
    }
}