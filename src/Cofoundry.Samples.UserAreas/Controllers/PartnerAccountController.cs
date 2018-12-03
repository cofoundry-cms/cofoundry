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
    [Route("partner/account")]
    [AuthorizeUserArea(PartnerUserAreaDefinition.Code)]
    public class PartnerAccountController : Controller
    {
        private static IUserAreaDefinition USER_AREA = new PartnerUserAreaDefinition();

        private readonly AuthenticationControllerHelper _authenticationControllerHelper;

        public PartnerAccountController(AuthenticationControllerHelper authenticationControllerHelper)
        {
            _authenticationControllerHelper = authenticationControllerHelper;
        }

        [Route("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
