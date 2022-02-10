using Cofoundry.Domain;
using Cofoundry.Web;
using Cofoundry.Web.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Samples.UserAreas
{
    [Route("partners/account")]
    [AuthorizeRole(PartnerUserArea.Code, PartnerRole.Code)]
    public class PartnerAccountController : Controller
    {
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
