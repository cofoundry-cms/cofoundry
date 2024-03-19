﻿using Cofoundry.Domain.Internal;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin;

public class NestedDataModelSchemaApiController : BaseAdminApiController
{
    private readonly IDomainRepository _domainRepository;
    private readonly IApiResponseHelper _apiResponseHelper;
    private readonly IModelValidationService _modelValidationService;
    private readonly DynamicDataModelJsonSerializerSettingsCache _dynamicDataModelSchemaJsonSerializerSettingsCache;

    public NestedDataModelSchemaApiController(
        IDomainRepository domainRepository,
        IApiResponseHelper apiResponseHelper,
        IModelValidationService modelValidationService,
        DynamicDataModelJsonSerializerSettingsCache dynamicDataModelSchemaJsonSerializerSettingsCache
        )
    {
        _domainRepository = domainRepository;
        _apiResponseHelper = apiResponseHelper;
        _modelValidationService = modelValidationService;
        _dynamicDataModelSchemaJsonSerializerSettingsCache = dynamicDataModelSchemaJsonSerializerSettingsCache;
    }

    public async Task<JsonResult> Get([FromQuery] GetNestedDataModelSchemaByNameRangeQuery rangeQuery)
    {
        if (EnumerableHelper.IsNullOrEmpty(rangeQuery.Names))
        {
            return _apiResponseHelper.SimpleQueryResponse(Enumerable.Empty<CustomEntityDataModelSchema>());
        }

        var jsonResponse = await _apiResponseHelper.RunWithResultAsync(async () =>
        {
            return await _domainRepository
                .WithQuery(rangeQuery)
                .FilterAndOrderByKeys(rangeQuery.Names)
                .ExecuteAsync();
        });
        jsonResponse.SerializerSettings = _dynamicDataModelSchemaJsonSerializerSettingsCache.GetInstance();

        return jsonResponse;
    }

    public async Task<JsonResult> GetByName(string dataModelName)
    {
        var jsonResponse = await _apiResponseHelper.RunQueryAsync(new GetNestedDataModelSchemaByNameQuery(dataModelName));
        jsonResponse.SerializerSettings = _dynamicDataModelSchemaJsonSerializerSettingsCache.GetInstance();

        return jsonResponse;
    }

    public JsonResult Validate([ModelBinder(BinderType = typeof(NestedDataModelMultiTypeItemModelBinder))] NestedDataModelMultiTypeItem item)
    {
        if (item?.Model == null)
        {
            throw new Exception("Error binding model");
        }

        var errors = _modelValidationService
            .GetErrors(item.Model)
            .ToArray();

        return _apiResponseHelper.SimpleCommandResponse(errors);
    }
}
