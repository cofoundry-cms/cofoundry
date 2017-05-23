using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    [AdminAuthorize]
    [Route(RouteConstants.AdminAreaPrefix)]
    public class BaseAdminMvcController : Controller
    {
    }
}