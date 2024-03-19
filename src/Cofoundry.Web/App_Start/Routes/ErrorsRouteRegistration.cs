﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Cofoundry.Web;

public class ErrorsRouteRegistration : IOrderedRouteRegistration, IRunAfterRouteRegistration
{
    public int Ordering => (int)RouteRegistrationOrdering.Early;

    public IReadOnlyCollection<Type> RunAfter => [typeof(AssetsRouteRegistration)];

    public void RegisterRoutes(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapControllerRoute(
            "Cofoundry_ErrorCode",
            "cofoundryerror/errorcode/{statusCode}",
            new { controller = "CofoundryError", action = "ErrorCode" },
            new { statusCode = @"\d+" });

        routeBuilder.MapControllerRoute(
            "Cofoundry_Exception",
            "cofoundryerror/exception/",
            new { controller = "CofoundryError", action = "Exception" });
    }
}
