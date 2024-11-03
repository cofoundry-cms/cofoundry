using Cofoundry.Plugins.YouTube.Domain;
using Cofoundry.Web;
using Cofoundry.Web.Admin;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Plugins.YouTube.Admin.Plugins.Admin.Api;

public class YouTubeSettingsApiController : BaseAdminApiController
{
    private readonly YouTubeSettings _youtubeSettings;
    private readonly IApiResponseHelper _apiResponseHelper;

    public YouTubeSettingsApiController(
        YouTubeSettings youtubeSettings,
        IApiResponseHelper apiResponseHelper
        )
    {
        _youtubeSettings = youtubeSettings;
        _apiResponseHelper = apiResponseHelper;
    }

    public IActionResult Get()
    {
        return _apiResponseHelper.SimpleQueryResponse(_youtubeSettings);
    }
}
