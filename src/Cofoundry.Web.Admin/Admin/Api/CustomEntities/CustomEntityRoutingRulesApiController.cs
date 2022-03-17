using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin;

public class CustomEntityRoutingRulesApiController : BaseAdminApiController
{
    private readonly IApiResponseHelper _apiResponseHelper;

    public CustomEntityRoutingRulesApiController(
        IApiResponseHelper apiResponseHelper
        )
    {
        _apiResponseHelper = apiResponseHelper;
    }

    public async Task<JsonResult> Get()
    {
        return await _apiResponseHelper.RunQueryAsync(new GetAllCustomEntityRoutingRulesQuery());
    }
}
