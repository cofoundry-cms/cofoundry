using Cofoundry.Web;
using Cofoundry.Web.Admin;
using Microsoft.AspNetCore.Routing;

namespace Cofoundry.Plugins.YouTube.Admin.Plugins.Admin.Api;

public class YouTubeApiRouteRegistration : IRouteRegistration
{
    public void RegisterRoutes(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder
            .ForAdminApiController<YouTubeSettingsApiController>("plugins/youtube-settings")
            .MapGet();
    }
}
