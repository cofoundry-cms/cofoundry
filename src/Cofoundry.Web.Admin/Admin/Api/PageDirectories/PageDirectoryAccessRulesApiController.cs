using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cofoundry.Web.Admin
{
    public class PageDirectoryAccessRulesApiController : BaseAdminApiController
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;

        public PageDirectoryAccessRulesApiController(
            IQueryExecutor queryExecutor,
            IApiResponseHelper apiResponseHelper
            )
        {
            _queryExecutor = queryExecutor;
            _apiResponseHelper = apiResponseHelper;
        }

        public async Task<JsonResult> Get(int pageDirectoryId)
        {
            var query = new GetPageDirectoryAccessDetailsByPageDirectoryIdQuery(pageDirectoryId);

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(results);
        }

        public Task<JsonResult> Patch(int pageDirectoryId, [FromBody] IDelta<UpdatePageDirectoryAccessRuleSetCommand> delta)
        {
            return _apiResponseHelper.RunCommandAsync(pageDirectoryId, delta);
        }
    }
}