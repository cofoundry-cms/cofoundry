using Cofoundry.Core;
using Cofoundry.Core.Validation;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web.Admin
{
    public class NestedDataModelSchemaApiController : BaseAdminApiController
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;
        private readonly IModelValidationService _modelValidationService;

        public NestedDataModelSchemaApiController(
            IQueryExecutor queryExecutor,
            IApiResponseHelper apiResponseHelper,
            IModelValidationService modelValidationService
            )
        {
            _queryExecutor = queryExecutor;
            _apiResponseHelper = apiResponseHelper;
            _modelValidationService = modelValidationService;
        }

        public async Task<IActionResult> Get([FromQuery] GetNestedDataModelSchemaByNameRangeQuery rangeQuery)
        {
            if (EnumerableHelper.IsNullOrEmpty(rangeQuery.Names))
            {
                return _apiResponseHelper.SimpleQueryResponse(this, Enumerable.Empty<CustomEntityDataModelSchema>());
            }
            var result = await _queryExecutor.ExecuteAsync(rangeQuery);
            return _apiResponseHelper.SimpleQueryResponse(this, result.FilterAndOrderByKeys(rangeQuery.Names));
        }
        
        public async Task<IActionResult> GetByName(string dataModelName)
        {
            var results = await _queryExecutor.ExecuteAsync(new GetNestedDataModelSchemaByNameQuery(dataModelName));
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        public IActionResult Validate([ModelBinder(BinderType = typeof(NestedDataModelMultiTypeItemModelBinder))] NestedDataModelMultiTypeItem item)
        {
            if (item?.Model == null)
            {
                throw new Exception("Error binding model");
            }

            var errors = _modelValidationService.GetErrors(item.Model).ToList();

            return _apiResponseHelper.SimpleCommandResponse(this, errors);
        }
    }
}
