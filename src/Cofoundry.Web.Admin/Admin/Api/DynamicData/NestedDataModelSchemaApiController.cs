using Cofoundry.Core;
using Cofoundry.Core.Validation;
using Cofoundry.Domain;
using Cofoundry.Domain.Internal;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web.Admin
{
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

            var errors = _modelValidationService.GetErrors(item.Model).ToList();

            return _apiResponseHelper.SimpleCommandResponse(errors);
        }
    }
}