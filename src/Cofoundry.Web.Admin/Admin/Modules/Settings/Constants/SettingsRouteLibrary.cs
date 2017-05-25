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

        public SettingsRouteLibrary(
            IStaticResourceFileProvider staticResourceFileProvider,
            OptimizationSettings optimizationSettings
            )
            : base(
                  RoutePrefix,
                  RouteConstants.InternalModuleResourcePathPrefix,
                  staticResourceFileProvider,
                  optimizationSettings
                  )
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