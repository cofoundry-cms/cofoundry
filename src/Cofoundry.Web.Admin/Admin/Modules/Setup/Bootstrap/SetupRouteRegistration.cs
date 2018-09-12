using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Web.Admin
{
    public class SetupRouteRegistration : IOrderedRouteRegistration
    {
        public int Ordering => (int)RouteRegistrationOrdering.Early;

        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute(
                "Cofoundry Admin Module - Setup",
                RouteConstants.AdminAreaPrefix + "/" + SetupRouteLibrary.RoutePrefix,
                new { controller = "Setup", action = "Index", Area = RouteConstants.AdminAreaName }
                );
        }
    }
}
