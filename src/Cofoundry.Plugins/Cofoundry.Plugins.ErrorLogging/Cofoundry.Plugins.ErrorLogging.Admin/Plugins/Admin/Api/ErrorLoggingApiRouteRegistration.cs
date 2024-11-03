using Cofoundry.Web;
using Cofoundry.Web.Admin;
using Microsoft.AspNetCore.Routing;

namespace Cofoundry.Plugins.ErrorLogging.Admin;

public class ErrorLoggingApiRouteRegistration : IOrderedRouteRegistration
{
    public int Ordering => (int)RouteRegistrationOrdering.Early;

    public void RegisterRoutes(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder
            .ForAdminApiController<ErrorsApiController>("plugins/errors")
            .MapGet()
            .MapGetById("{errorId:int}")
            ;
    }
}
