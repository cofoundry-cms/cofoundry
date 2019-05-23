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
    public class NestedDataModelSchemaApiController : BaseAdminApiController
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;

        public NestedDataModelSchemaApiController(
            IQueryExecutor queryExecutor,
            IApiResponseHelper apiResponseHelper
            )
        {
            _queryExecutor = queryExecutor;
            _apiResponseHelper = apiResponseHelper;
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
    }
}
