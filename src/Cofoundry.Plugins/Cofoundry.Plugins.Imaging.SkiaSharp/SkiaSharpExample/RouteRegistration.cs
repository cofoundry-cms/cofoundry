using Cofoundry.Web;

namespace SkiaSharpExample;

public class RouteRegistration : IRouteRegistration
{
    public void RegisterRoutes(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapRazorPages();
    }
}
