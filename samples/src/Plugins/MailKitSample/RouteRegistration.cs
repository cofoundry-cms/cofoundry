using Cofoundry.Web;

namespace MailKitSample;

public class RouteRegistration : IRouteRegistration
{
    public void RegisterRoutes(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapRazorPages();
    }
}
