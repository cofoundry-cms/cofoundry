using Cofoundry.Web;

namespace MailKitExample;

public class RouteRegistration : IRouteRegistration
{
    public void RegisterRoutes(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapRazorPages();
    }
}
