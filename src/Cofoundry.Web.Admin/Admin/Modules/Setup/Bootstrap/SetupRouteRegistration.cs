using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Web.Admin
{
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
}
