using Cofoundry.Web;

namespace ImageSharpSample;

public class RouteRegistration : IRouteRegistration
{
    public void RegisterRoutes(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapRazorPages();
    }
}
