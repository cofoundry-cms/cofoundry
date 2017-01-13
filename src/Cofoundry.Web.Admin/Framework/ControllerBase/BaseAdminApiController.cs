using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Cofoundry.Web.WebApi;

namespace Cofoundry.Web.Admin
{
    [AdminApiAuthorize]
    [ValidateApiAntiForgeryToken]
    public class BaseAdminApiController : ApiController
    {
    }
}