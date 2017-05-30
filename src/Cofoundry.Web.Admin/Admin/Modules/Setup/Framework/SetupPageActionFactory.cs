using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// Returns the route to the Cofoundry setup screen in the
    /// admin panel.
    /// </summary>
    public class SetupPageActionFactory : ISetupPageActionFactory
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IAdminRouteLibrary _adminRouteLibrary;

        public SetupPageActionFactory(
            IQueryExecutor queryExecutor,
            IAdminRouteLibrary adminRouteLibrary
            )
        {
            _queryExecutor = queryExecutor;
            _adminRouteLibrary = adminRouteLibrary;
        }

        public ActionResult GetSetupPageAction(Controller controller)
        {
            return controller.Redirect(_adminRouteLibrary.Setup.Setup());
        }
    }
}