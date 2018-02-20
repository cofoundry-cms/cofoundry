using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin
{
    public class StandardAngularModuleController : BaseAdminMvcController
    {
        public ActionResult Index()
        {

            var vm = new StandardAngularModuleViewModel();
            vm.RouteLibrary = RouteData.DataTokens["RouteLibrary"] as AngularModuleRouteLibrary;
            if (vm.RouteLibrary == null) throw new Exception("The 'RouteLibrary' route data token should not be null.");

            vm.Title = RouteData.DataTokens["ModuleTitle"] as string;
            if (vm.RouteLibrary == null) throw new Exception("The 'ModuleTitle' route data token should not be null.");

            return View("~/Framework/Angular/StandardAngularModules/Routes/Index.cshtml", vm);
        }
    }
}