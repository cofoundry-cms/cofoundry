using Cofoundry.Core;
using Cofoundry.Core.Json;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Internal;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web.Admin
{
    public class CustomEntityDataModelSchemaApiController : BaseAdminApiController
    {
        private readonly IDomainRepository _domainRepository;
        private readonly IApiResponseHelper _apiResponseHelper;
        private readonly DynamicDataModelJsonSerializerSettingsCache _dynamicDataModelSchemaJsonSerializerSettingsCache;

        public CustomEntityDataModelSchemaApiController(
            IDomainRepository domainRepository,
            IApiResponseHelper apiResponseHelper,
            DynamicDataModelJsonSerializerSettingsCache dynamicDataModelSchemaJsonSerializerSettingsCache
            )
        {
            _domainRepository = domainRepository;
            _apiResponseHelper = apiResponseHelper;
            _dynamicDataModelSchemaJsonSerializerSettingsCache = dynamicDataModelSchemaJsonSerializerSettingsCache;
        }

        public async Task<JsonResult> Get([FromQuery] GetCustomEntityDataModelSchemaDetailsByDefinitionCodeRangeQuery rangeQuery)
        {
            if (rangeQuery.CustomEntityDefinitionCodes == null)
            {
                return _apiResponseHelper.SimpleQueryResponse(Enumerable.Empty<CustomEntityDataModelSchema>());
            }

            var result = await _domainRepository
                .WithQuery(rangeQuery)
                .FilterAndOrderByKeys(rangeQuery.CustomEntityDefinitionCodes)
                .ExecuteAsync();

            var settings = _dynamicDataModelSchemaJsonSerializerSettingsCache.GetInstance();
            var jsonResponse = _apiResponseHelper.SimpleQueryResponse(result);
            jsonResponse.SerializerSettings = settings;

            return jsonResponse;
        }

        public async Task<JsonResult> GetDataModelSchema(string customEntityDefinitionCode)
        {
            var result = await _domainRepository.ExecuteQueryAsync(new GetCustomEntityDataModelSchemaDetailsByDefinitionCodeQuery(customEntityDefinitionCode));
            var settings = _dynamicDataModelSchemaJsonSerializerSettingsCache.GetInstance();
            var jsonResponse = _apiResponseHelper.SimpleQueryResponse(result);
            jsonResponse.SerializerSettings = settings;

            return jsonResponse;
        }
    }
}
