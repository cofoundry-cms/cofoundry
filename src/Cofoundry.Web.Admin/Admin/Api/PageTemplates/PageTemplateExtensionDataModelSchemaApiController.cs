using Cofoundry.Domain.Internal;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin;

public class PageTemplateExtensionDataModelSchemaApiController : BaseAdminApiController
{
    private readonly IDomainRepository _domainRepository;
    private readonly IApiResponseHelper _apiResponseHelper;
    private readonly IModelValidationService _modelValidationService;
    private readonly DynamicDataModelJsonSerializerSettingsCache _dynamicDataModelSchemaJsonSerializerSettingsCache;

    public PageTemplateExtensionDataModelSchemaApiController(
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

    public async Task<JsonResult> Get(int pageTemplateId)
    {
        var jsonResponse = await _apiResponseHelper.RunQueryAsync(new GetPageExtensionDataModelSchemasByPageTemplateIdQuery(pageTemplateId));
        jsonResponse.SerializerSettings = _dynamicDataModelSchemaJsonSerializerSettingsCache.GetInstance();

        return jsonResponse;
    }
}
