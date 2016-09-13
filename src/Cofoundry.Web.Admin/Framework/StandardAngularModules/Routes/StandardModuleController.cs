using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Web.Admin
{
    public class StandardModuleController : BaseAdminMvcController
    {
        public ActionResult Index(ModuleRouteLibrary routeLibrary)
        {
            var jsRouteLibrary = new ModuleJsRouteLibrary(routeLibrary);
            return View("~/Framework/StandardAngularModules/Routes/index.cshtml", jsRouteLibrary);
        }
    }
}