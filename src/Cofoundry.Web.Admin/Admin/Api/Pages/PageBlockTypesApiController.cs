using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Web.Admin
{
    public class PageBlockTypesApiController : BaseAdminApiController
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;
        private readonly DynamicDataModelJsonSerializerSettingsCache _dynamicDataModelSchemaJsonSerializerSettingsCache;

        public PageBlockTypesApiController(
            IQueryExecutor queryExecutor,
            IApiResponseHelper apiResponseHelper,
            DynamicDataModelJsonSerializerSettingsCache dynamicDataModelSchemaJsonSerializerSettingsCache
            )
        {
            _queryExecutor = queryExecutor;
            _apiResponseHelper = apiResponseHelper;
            _dynamicDataModelSchemaJsonSerializerSettingsCache = dynamicDataModelSchemaJsonSerializerSettingsCache;
        }

        public async Task<JsonResult> Get()
        {
            var results = await _queryExecutor.ExecuteAsync(new GetAllPageBlockTypeSummariesQuery());
            return _apiResponseHelper.SimpleQueryResponse(results);
        }

        public async Task<JsonResult> GetById(int pageBlockTypeId)
        {
            var result = await _queryExecutor.ExecuteAsync(new GetPageBlockTypeDetailsByIdQuery(pageBlockTypeId));

            var settings = _dynamicDataModelSchemaJsonSerializerSettingsCache.GetInstance();
            var jsonResponse = _apiResponseHelper.SimpleQueryResponse(result);
            jsonResponse.SerializerSettings = settings;

            return jsonResponse;
        }
    }
}