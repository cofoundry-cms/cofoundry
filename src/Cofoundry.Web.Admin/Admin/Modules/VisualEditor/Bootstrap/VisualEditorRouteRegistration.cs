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
            routeBuilder
                .ForAdminController<VisualEditorController>(VisualEditorRouteLibrary.RoutePrefix + "/frame")
                .MapRoute("Frame");
        }
    }
}
