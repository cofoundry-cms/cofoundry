using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin
{
    [Area(RouteConstants.AdminAreaName)]
    [AdminAuthorize]
    [AutoValidateAntiforgeryToken]
    public class BaseAdminApiController : ControllerBase
    {
    }
}