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
    [AuthorizeRole(PartnerUserAreaDefinition.Code, PartnerRoleDefinition.Code)]
    public class PartnerAccountController : Controller
    {
        private readonly IAuthenticationControllerHelper<PartnerUserAreaDefinition> _authenticationControllerHelper;

        public PartnerAccountController(
            IAuthenticationControllerHelper<PartnerUserAreaDefinition> authenticationControllerHelper
            )
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
