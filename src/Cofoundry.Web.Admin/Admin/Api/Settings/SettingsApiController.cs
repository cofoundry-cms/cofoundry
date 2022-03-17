using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin;

public class SettingsApiController : BaseAdminApiController
{
    private readonly IApiResponseHelper _apiResponseHelper;

    public SettingsApiController(
        IApiResponseHelper apiResponseHelper
        )
    {
        _apiResponseHelper = apiResponseHelper;
    }

    public async Task<IActionResult> GetGeneralSiteSettings()
    {
        var query = new GetSettingsQuery<GeneralSiteSettings>();
        return await _apiResponseHelper.RunQueryAsync(query);
    }

    public async Task<IActionResult> GetSeoSettings()
    {
        var query = new GetSettingsQuery<SeoSettings>();
        return await _apiResponseHelper.RunQueryAsync(query);
    }

    public Task<JsonResult> PatchGeneralSiteSettings([FromBody] IDelta<UpdateGeneralSiteSettingsCommand> delta)
    {
        return _apiResponseHelper.RunCommandAsync(delta);
    }

    public Task<JsonResult> PatchSeoSettings([FromBody] IDelta<UpdateSeoSettingsCommand> delta)
    {
        return _apiResponseHelper.RunCommandAsync(delta);
    }
}
