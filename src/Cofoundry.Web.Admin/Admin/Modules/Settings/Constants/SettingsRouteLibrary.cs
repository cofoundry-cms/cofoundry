using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Admin
{
    public class SettingsRouteLibrary : ModuleRouteLibrary
    {
        public const string RoutePrefix = "settings";

        #region constructor

        public SettingsRouteLibrary(AdminSettings adminSettings)
            : base(adminSettings, RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
        {
        }

        #endregion

        #region routes

        public string Details()
        {
            return AngularRoute();
        }

        #endregion
    }
}