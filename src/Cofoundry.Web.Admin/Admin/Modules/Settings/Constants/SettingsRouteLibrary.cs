using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class SettingsRouteLibrary : ModuleRouteLibrary
    {
        public const string RoutePrefix = "settings";

        #region constructor

        public SettingsRouteLibrary()
            : base(RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
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