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

        public SetupController(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
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