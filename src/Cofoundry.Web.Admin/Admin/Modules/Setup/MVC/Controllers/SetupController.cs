using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cofoundry.Web.Admin
{
    [Area(RouteConstants.AdminAreaName)]
    public class SetupController : Controller
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IUserSignInService _signInService;
        private readonly IAdminRouteLibrary _adminRouteLibrary;

        public SetupController(
            IQueryExecutor queryExecutor,
            IUserSignInService signInService,
            IAdminRouteLibrary adminRouteLibrary
            )
        {
            _queryExecutor = queryExecutor;
            _signInService = signInService;
            _adminRouteLibrary = adminRouteLibrary;
        }

        public async Task<ActionResult> Index()
        {
            var settings = await _queryExecutor.ExecuteAsync(new GetSettingsQuery<InternalSettings>());
            if (settings.IsSetup)
            {
                return RedirectToDashboard();
            }

            // force sign-out - solves a rare case where you're re-initializing a db after being signed into a previous version.
            await _signInService.SignOutAllUserAreasAsync();

            var viewPath = ViewPathFormatter.View("Setup", nameof(Index));
            return View(viewPath);
        }

        private ActionResult RedirectToDashboard()
        {
            return Redirect(_adminRouteLibrary.Dashboard.Dashboard());
        }
    }
}