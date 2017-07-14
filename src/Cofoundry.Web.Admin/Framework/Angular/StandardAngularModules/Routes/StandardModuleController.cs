using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin
{
    public class StandardModuleController : BaseAdminMvcController
    {
        public ActionResult Index()
        {
            var routeLibrary = RouteData.DataTokens["RouteLibrary"] as AngularModuleRouteLibrary;
            if (routeLibrary == null) throw new InvalidOperationException("The 'RouteLibrary' route data token should not be null.");

            return View("~/Framework/Angular/StandardAngularModules/Routes/Index.cshtml", routeLibrary);
        }
    }
}