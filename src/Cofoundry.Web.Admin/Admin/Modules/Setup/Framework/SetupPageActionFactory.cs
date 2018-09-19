using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// Returns the route to the Cofoundry setup screen in the
    /// admin panel.
    /// </summary>
    public class SetupPageActionFactory : ISetupPageActionFactory
    {
        private readonly AdminSettings _adminSettings;
        private readonly IAdminRouteLibrary _adminRouteLibrary;

        public SetupPageActionFactory(
            AdminSettings adminSettings,
            IAdminRouteLibrary adminRouteLibrary
            )
        {
            _adminSettings = adminSettings;
            _adminRouteLibrary = adminRouteLibrary;
        }

        public ActionResult GetSetupPageAction(Controller controller)
        {
            if (_adminSettings.Disabled)
            {
                throw new Exception("Cofoundry has not been setup. The admin panel is disabled so installation must be done manually.");
            }

            return controller.Redirect(_adminRouteLibrary.Setup.Setup());
        }
    }
}