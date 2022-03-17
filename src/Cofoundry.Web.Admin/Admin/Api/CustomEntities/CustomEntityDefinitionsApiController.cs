using Cofoundry.Domain.Internal;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin;

public class CustomEntityDefinitionsApiController : BaseAdminApiController
{
    private readonly IApiResponseHelper _apiResponseHelper;
    private readonly DynamicDataModelJsonSerializerSettingsCache _dynamicDataModelSchemaJsonSerializerSettingsCache;

    public CustomEntityDefinitionsApiController(
        IApiResponseHelper apiResponseHelper,
        DynamicDataModelJsonSerializerSettingsCache dynamicDataModelSchemaJsonSerializerSettingsCache
        )
    {
        _apiResponseHelper = apiResponseHelper;
        _dynamicDataModelSchemaJsonSerializerSettingsCache = dynamicDataModelSchemaJsonSerializerSettingsCache;
    }

    public async Task<JsonResult> Get()
    {
        return await _apiResponseHelper.RunQueryAsync(new GetAllCustomEntityDefinitionSummariesQuery());
    }

    public async Task<JsonResult> GetById(string customEntityDefinitionCode)
    {
        return await _apiResponseHelper.RunQueryAsync(new GetCustomEntityDefinitionSummaryByCodeQuery(customEntityDefinitionCode));
    }

    public async Task<JsonResult> GetCustomEntityRoutes(string customEntityDefinitionCode)
    {
        return await _apiResponseHelper.RunQueryAsync(new GetPageRoutesByCustomEntityDefinitionCodeQuery(customEntityDefinitionCode));
    }

    public async Task<JsonResult> GetDataModelSchema(string customEntityDefinitionCode)
    {
        var query = new GetCustomEntityDataModelSchemaDetailsByDefinitionCodeQuery(customEntityDefinitionCode);
        var jsonResponse = await _apiResponseHelper.RunQueryAsync(query);

        var settings = _dynamicDataModelSchemaJsonSerializerSettingsCache.GetInstance();
        jsonResponse.SerializerSettings = settings;

        return jsonResponse;
    }

    public async Task<JsonResult> GetCustomEntities(string customEntityDefinitionCode, [FromQuery] SearchCustomEntitySummariesQuery query)
    {
        if (query == null) query = new SearchCustomEntitySummariesQuery();
        query.CustomEntityDefinitionCode = customEntityDefinitionCode;
        ApiPagingHelper.SetDefaultBounds(query);

        return await _apiResponseHelper.RunQueryAsync(query);
    }
}
