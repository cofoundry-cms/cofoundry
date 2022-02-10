using Cofoundry.Web;
using Microsoft.AspNetCore.Mvc;

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