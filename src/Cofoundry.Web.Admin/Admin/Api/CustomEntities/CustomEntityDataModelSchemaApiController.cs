﻿using Cofoundry.Domain.Internal;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin;

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
        if (EnumerableHelper.IsNullOrEmpty(rangeQuery.CustomEntityDefinitionCodes))
        {
            return _apiResponseHelper.SimpleQueryResponse(Enumerable.Empty<CustomEntityDataModelSchema>());
        }

        var jsonResponse = await _apiResponseHelper.RunWithResultAsync(async () =>
        {
            return await _domainRepository
                .WithQuery(rangeQuery)
                .FilterAndOrderByKeys(rangeQuery.CustomEntityDefinitionCodes)
                .ExecuteAsync();
        });

        var settings = _dynamicDataModelSchemaJsonSerializerSettingsCache.GetInstance();
        jsonResponse.SerializerSettings = settings;

        return jsonResponse;
    }

    public async Task<JsonResult> GetDataModelSchema(string customEntityDefinitionCode)
    {
        var query = new GetCustomEntityDataModelSchemaDetailsByDefinitionCodeQuery(customEntityDefinitionCode);
        var jsonResponse = await _apiResponseHelper.RunQueryAsync(query);

        var settings = _dynamicDataModelSchemaJsonSerializerSettingsCache.GetInstance();
        jsonResponse.SerializerSettings = settings;

        return jsonResponse;
    }
}
