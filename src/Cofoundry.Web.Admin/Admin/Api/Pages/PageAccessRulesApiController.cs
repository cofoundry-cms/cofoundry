using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cofoundry.Web.Admin
{
    public class PageAccessRulesApiController : BaseAdminApiController
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;

        public PageAccessRulesApiController(
            IQueryExecutor queryExecutor,
            IApiResponseHelper apiResponseHelper
            )
        {
            _queryExecutor = queryExecutor;
            _apiResponseHelper = apiResponseHelper;
        }

        public async Task<JsonResult> Get(int pageId)
        {
            var query = new GetPageAccessInfoByPageIdQuery(pageId);

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(results);
        }

        public Task<JsonResult> Patch(int pageId, [FromBody] IDelta<UpdatePageAccessRulesCommand> delta)
        {
            return _apiResponseHelper.RunCommandAsync(pageId, delta);
        }
    }
}