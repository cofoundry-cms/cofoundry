using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Web.WebApi;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin
{
    [AdminAuthorize]
    [ValidateApiAntiForgeryToken]
    public class BaseAdminApiController : Controller
    {
    }
}