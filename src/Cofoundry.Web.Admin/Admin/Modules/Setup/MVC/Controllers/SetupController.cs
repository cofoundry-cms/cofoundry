using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Web.Admin
{
    [RouteArea(RouteConstants.AdminAreaName, AreaPrefix = RouteConstants.AdminAreaPrefix)]
    [RoutePrefix(SetupRouteLibrary.RoutePrefix)]
    [Route("{action=index}")]
    public class SetupController : Controller
    {
        #region Constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly ILoginService _loginService;

        public SetupController(
            IQueryExecutor queryExecutor,
            ILoginService loginService
            )
        {
            _queryExecutor = queryExecutor;
            _loginService = loginService;
        }

        #endregion

        #region routes

        [HttpGet]
        public ActionResult Index()
        {
            var settings = _queryExecutor.Get<InternalSettings>();
            if (settings.IsSetup)
            {
                return RedirectToDashboard();
            }

            // force sign-out - solves a rare case where you're re-initializing a db after being signed into a previous version.
            _loginService.SignOut();

            return View();
        }

        #endregion

        #region private helpers

        private ActionResult RedirectToDashboard()
        {
            return Redirect(DashboardRouteLibrary.Urls.Dashboard());
        }

        #endregion
    }
}