using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Cofoundry.Web
{
    public class RobotsRouteRegistration : IOrderedRouteRegistration
    {
        public int Ordering => (int)RouteRegistrationOrdering.Early;

        public void RegisterRoutes(IEndpointRouteBuilder routeBuilder)
        {
            // General files
            routeBuilder.MapControllerRoute("RobotsTxt", "robots.txt", new { controller = "CofoundryFiles", action = "RobotsTxt" });
            routeBuilder.MapControllerRoute("HumansTxt", "humans.txt", new { controller = "CofoundryFiles", action = "HumansTxt" });
        }
    }
}
