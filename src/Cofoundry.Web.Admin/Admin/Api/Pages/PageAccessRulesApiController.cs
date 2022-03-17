using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin;

public class PageAccessRulesApiController : BaseAdminApiController
{
    private readonly IApiResponseHelper _apiResponseHelper;

    public PageAccessRulesApiController(
        IApiResponseHelper apiResponseHelper
        )
    {
        _apiResponseHelper = apiResponseHelper;
    }

    public async Task<JsonResult> Get(int pageId)
    {
        var query = new GetPageAccessRuleSetDetailsByPageIdQuery(pageId);
        return await _apiResponseHelper.RunQueryAsync(query);
    }

    public Task<JsonResult> Patch(int pageId, [FromBody] IDelta<UpdatePageAccessRuleSetCommand> delta)
    {
        return _apiResponseHelper.RunCommandAsync(pageId, delta);
    }
}
