using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class DashboardRouteLibrary : AngularModuleRouteLibrary
    {
        #region statics

        public const string RoutePrefix = "dashboard";

        #endregion

        #region constructor

        public DashboardRouteLibrary(
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

        public string Dashboard()
        {
            return AngularRoute();
        }

        #endregion
    }
}