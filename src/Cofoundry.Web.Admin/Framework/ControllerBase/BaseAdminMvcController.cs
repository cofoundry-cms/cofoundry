using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin;

[Area(RouteConstants.AdminAreaName)]
[AdminAuthorize]
public class BaseAdminMvcController : Controller
{
}
