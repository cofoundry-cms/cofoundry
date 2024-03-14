using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin;

public class StandardAngularModuleController : BaseAdminMvcController
{
    public ActionResult Index()
    {

        var routeLibrary = RouteData.DataTokens["RouteLibrary"] as AngularModuleRouteLibrary;
        if (routeLibrary == null)
        {
            throw new Exception("The 'RouteLibrary' route data token should not be null.");
        }

        var title = RouteData.DataTokens["ModuleTitle"] as string;
        if (title == null)
        {
            throw new Exception("The 'ModuleTitle' route data token should not be null.");
        }

        var vm = new StandardAngularModuleViewModel()
        {
            RouteLibrary = routeLibrary,
            Title = title
        };

        return View("~/Framework/Angular/StandardAngularModules/Routes/Index.cshtml", vm);
    }
}
