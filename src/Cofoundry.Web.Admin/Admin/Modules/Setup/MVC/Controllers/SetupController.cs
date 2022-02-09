using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cofoundry.Web.Admin
{
    [Area(RouteConstants.AdminAreaName)]
    public class SetupController : Controller
    {
        private readonly IDomainRepository _domainRepository;
        private readonly IAdminRouteLibrary _adminRouteLibrary;

        public SetupController(
            IDomainRepository domainRepository,
            IAdminRouteLibrary adminRouteLibrary
            )
        {
            _domainRepository = domainRepository;
            _adminRouteLibrary = adminRouteLibrary;
        }

        public async Task<ActionResult> Index()
        {
            var settings = await _domainRepository.ExecuteQueryAsync(new GetSettingsQuery<InternalSettings>());
            if (settings.IsSetup)
            {
                return RedirectToDashboard();
            }

            // force sign-out - solves a rare case where you're re-initializing a db after being signed into a previous version.
            await _domainRepository.ExecuteCommandAsync(new SignOutCurrentUserFromAllUserAreasCommand());

            var viewPath = ViewPathFormatter.View("Setup", nameof(Index));
            return View(viewPath);
        }

        private ActionResult RedirectToDashboard()
        {
            return Redirect(_adminRouteLibrary.Dashboard.Dashboard());
        }
    }
}