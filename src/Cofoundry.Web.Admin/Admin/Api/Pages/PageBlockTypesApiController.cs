using Cofoundry.Domain;
using Cofoundry.Domain.Internal;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cofoundry.Web.Admin
{
    public class PageBlockTypesApiController : BaseAdminApiController
    {
        private readonly IApiResponseHelper _apiResponseHelper;
        private readonly DynamicDataModelJsonSerializerSettingsCache _dynamicDataModelSchemaJsonSerializerSettingsCache;

        public PageBlockTypesApiController(
            IApiResponseHelper apiResponseHelper,
            DynamicDataModelJsonSerializerSettingsCache dynamicDataModelSchemaJsonSerializerSettingsCache
            )
        {
            _apiResponseHelper = apiResponseHelper;
            _dynamicDataModelSchemaJsonSerializerSettingsCache = dynamicDataModelSchemaJsonSerializerSettingsCache;
        }

        public async Task<JsonResult> Get()
        {
            var query = new GetAllPageBlockTypeSummariesQuery();
            return await _apiResponseHelper.RunQueryAsync(query);
        }

        public async Task<JsonResult> GetById(int pageBlockTypeId)
        {
            var query = new GetPageBlockTypeDetailsByIdQuery(pageBlockTypeId);
            var jsonResponse = await _apiResponseHelper.RunQueryAsync(query);
            jsonResponse.SerializerSettings = _dynamicDataModelSchemaJsonSerializerSettingsCache.GetInstance();

            return jsonResponse;
        }
    }
}