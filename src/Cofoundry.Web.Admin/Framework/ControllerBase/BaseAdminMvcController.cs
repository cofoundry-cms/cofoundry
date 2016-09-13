using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cofoundry.Web.Admin
{
    [AdminAuthorize]
    [RouteArea(RouteConstants.AdminAreaName, AreaPrefix = RouteConstants.AdminAreaPrefix)]
    [ExceptionLog(View = "~/Admin/Modules/Shared/MVC/Views/Error.cshtml")]
    public class BaseAdminMvcController : Controller
    {
    }
}