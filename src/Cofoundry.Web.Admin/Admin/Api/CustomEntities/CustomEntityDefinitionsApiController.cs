using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core;
using Cofoundry.Core.Json;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Internal;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin
{
    public class CustomEntityDefinitionsApiController : BaseAdminApiController
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;
        private readonly DynamicDataModelJsonSerializerSettingsCache _dynamicDataModelSchemaJsonSerializerSettingsCache;

        public CustomEntityDefinitionsApiController(
            IQueryExecutor queryExecutor,
            IApiResponseHelper apiResponseHelper,
            DynamicDataModelJsonSerializerSettingsCache dynamicDataModelSchemaJsonSerializerSettingsCache
            )
        {
            _queryExecutor = queryExecutor;
            _apiResponseHelper = apiResponseHelper;
            _dynamicDataModelSchemaJsonSerializerSettingsCache = dynamicDataModelSchemaJsonSerializerSettingsCache;
        }

        #region queries

        public async Task<JsonResult> Get()
        {
            var results = await _queryExecutor.ExecuteAsync(new GetAllCustomEntityDefinitionSummariesQuery());
            return _apiResponseHelper.SimpleQueryResponse(results);
        }

        public async Task<JsonResult> GetById(string customEntityDefinitionCode)
        {
            var result = await _queryExecutor.ExecuteAsync(new GetCustomEntityDefinitionSummaryByCodeQuery(customEntityDefinitionCode));
            return _apiResponseHelper.SimpleQueryResponse(result);
        }
        
        public async Task<JsonResult> GetCustomEntityRoutes(string customEntityDefinitionCode)
        {
            var query = new GetPageRoutesByCustomEntityDefinitionCodeQuery(customEntityDefinitionCode);
            var result = await _queryExecutor.ExecuteAsync(query);

            return _apiResponseHelper.SimpleQueryResponse(result);
        }

        public async Task<JsonResult> GetDataModelSchema(string customEntityDefinitionCode)
        {
            var result = await _queryExecutor.ExecuteAsync(new GetCustomEntityDataModelSchemaDetailsByDefinitionCodeQuery(customEntityDefinitionCode));
            var settings = _dynamicDataModelSchemaJsonSerializerSettingsCache.GetInstance();
            var jsonResponse = _apiResponseHelper.SimpleQueryResponse(result);
            jsonResponse.SerializerSettings = settings;

            return jsonResponse;
        }
        
        public async Task<JsonResult> GetCustomEntities(string customEntityDefinitionCode, [FromQuery] SearchCustomEntitySummariesQuery query)
        {
            if (query == null) query = new SearchCustomEntitySummariesQuery();
            query.CustomEntityDefinitionCode = customEntityDefinitionCode;
            ApiPagingHelper.SetDefaultBounds(query);

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(results);
        }

        #endregion
    }
}