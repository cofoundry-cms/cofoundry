using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Cofoundry.Web.Admin
{
    public class VisualEditorRouteRegistration : IOrderedRouteRegistration
    {
        public int Ordering => (int)RouteRegistrationOrdering.Early;

        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute(
                "Cofoundry Admin Module - Visual editor frame",
                RouteConstants.AdminAreaPrefix + "/" + VisualEditorRouteLibrary.RoutePrefix + "/frame",
                new { controller = "VisualEditor", action = "Frame", Area = RouteConstants.AdminAreaName }
                );
        }
    }
}
