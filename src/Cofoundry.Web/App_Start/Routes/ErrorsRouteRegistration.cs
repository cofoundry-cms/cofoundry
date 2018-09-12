using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Web
{
    public class ErrorsRouteRegistration : IOrderedRouteRegistration, IRunAfterRouteRegistration
    {
        public int Ordering => (int)RouteRegistrationOrdering.Early;

        public ICollection<Type> RunAfter => new Type[] { typeof(AssetsRouteRegistration) };

        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute(
                "Cofoundry_ErrorCode",
                "cofoundryerror/errorcode/{statusCode}",
                new { controller = "CofoundryError", action = "ErrorCode" },
                new { statusCode = @"\d+" });

            routeBuilder.MapRoute(
                "Cofoundry_Exception",
                "cofoundryerror/exception/",
                new { controller = "CofoundryError", action = "Exception" });
        }
    }
}
