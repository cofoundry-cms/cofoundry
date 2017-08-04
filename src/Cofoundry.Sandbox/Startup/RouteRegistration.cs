using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Cofoundry.Web;

namespace Cofoundry.Sandbox
{
    public class RouteRegistration : IRouteRegistration
    {
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute(
                "Home",
                "",
                new { controller = "Home", action = "Index" }
                );
            routeBuilder.MapRoute(
                "Home Actions",
                "Home/{action}"
                );
        }
    }
}
