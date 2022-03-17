using Microsoft.AspNetCore.Routing;

namespace Cofoundry.Web.Admin;

public class SetupRouteRegistration : IOrderedRouteRegistration
{
    public int Ordering => (int)RouteRegistrationOrdering.Early;

    public void RegisterRoutes(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder
            .ForAdminController<SetupController>(SetupRouteLibrary.RoutePrefix)
            .MapIndexRoute();
    }
}
