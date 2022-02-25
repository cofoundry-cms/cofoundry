using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cofoundry.Web.Admin
{
    public class PageDirectoryAccessRulesApiController : BaseAdminApiController
    {
        private readonly IApiResponseHelper _apiResponseHelper;

        public PageDirectoryAccessRulesApiController(
            IApiResponseHelper apiResponseHelper
            )
        {
            _apiResponseHelper = apiResponseHelper;
        }

        public async Task<JsonResult> Get(int pageDirectoryId)
        {
            var query = new GetPageDirectoryAccessDetailsByPageDirectoryIdQuery(pageDirectoryId);
            return await _apiResponseHelper.RunQueryAsync(query);
        }

        public Task<JsonResult> Patch(int pageDirectoryId, [FromBody] IDelta<UpdatePageDirectoryAccessRuleSetCommand> delta)
        {
            return _apiResponseHelper.RunCommandAsync(pageDirectoryId, delta);
        }
    }
}