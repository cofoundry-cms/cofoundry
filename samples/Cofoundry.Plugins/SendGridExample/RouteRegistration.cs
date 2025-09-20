namespace SendGridExample;

public class RouteRegistration : IRouteRegistration
{
    public void RegisterRoutes(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapRazorPages();
    }
}
