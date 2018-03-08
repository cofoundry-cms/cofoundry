using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web.Admin
{
    [AdminApiRoute("nested-data-model-schemas")]
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

        [HttpGet("{dataModelName}")]
        public async Task<IActionResult> Get(string dataModelName)
        {
            var results = await _queryExecutor.ExecuteAsync(new GetNestedDataModelSchemaByNameQuery(dataModelName));
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }
    }
}
