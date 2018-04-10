using Cofoundry.Core;
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
    [AdminApiRoute("custom-entity-data-model-schemas")]
    public class CustomEntityDataModelSchemaApiController : BaseAdminApiController
    {
        private const string ID_ROUTE = "{customEntityDefinitionCode}";

        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;

        public CustomEntityDataModelSchemaApiController(
            IQueryExecutor queryExecutor,
            IApiResponseHelper apiResponseHelper
            )
        {
            _queryExecutor = queryExecutor;
            _apiResponseHelper = apiResponseHelper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetCustomEntityDataModelSchemaDetailsByDefinitionCodeRangeQuery rangeQuery)
        {
            if (rangeQuery.CustomEntityDefinitionCodes == null)
            {
                return _apiResponseHelper.SimpleQueryResponse(this, Enumerable.Empty<CustomEntityDataModelSchema>());
            }
            var result = await _queryExecutor.ExecuteAsync(rangeQuery);
            return _apiResponseHelper.SimpleQueryResponse(this, result.FilterAndOrderByKeys(rangeQuery.CustomEntityDefinitionCodes));
        }

        [HttpGet(ID_ROUTE)]
        public async Task<IActionResult> GetDataModelSchema(string customEntityDefinitionCode)
        {
            var result = await _queryExecutor.ExecuteAsync(new GetCustomEntityDataModelSchemaDetailsByDefinitionCodeQuery(customEntityDefinitionCode));
            return _apiResponseHelper.SimpleQueryResponse(this, result);
        }
    }
}
