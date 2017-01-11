using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.OData;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Web.WebApi;

namespace Cofoundry.Web.Admin
{
    [AdminApiRoutePrefix("custom-entity-definitions")]
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
        [Route]
        public async Task<IHttpActionResult> Get()
        {
            var results = await _queryExecutor.GetAllAsync<CustomEntityDefinitionSummary>();
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        [HttpGet]
        [Route(ID_ROUTE)]
        public async Task<IHttpActionResult> Get(string customEntityDefinitionCode)
        {
            var result = await _queryExecutor.GetByIdAsync<CustomEntityDefinitionSummary>(customEntityDefinitionCode);
            return _apiResponseHelper.SimpleQueryResponse(this, result);
        }

        [HttpGet]
        [Route(ID_ROUTE + "/data-model-schema")]
        public async Task<IHttpActionResult> GetDataModelSchema(string customEntityDefinitionCode)
        {
            var result = await _queryExecutor.GetByIdAsync<CustomEntityDataModelSchema>(customEntityDefinitionCode);
            return _apiResponseHelper.SimpleQueryResponse(this, result);
        }

        
        [HttpGet]
        [Route(ID_ROUTE + "/custom-entities")]
        public async Task<IHttpActionResult> Get(string customEntityDefinitionCode, [FromUri] SearchCustomEntitySummariesQuery query)
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