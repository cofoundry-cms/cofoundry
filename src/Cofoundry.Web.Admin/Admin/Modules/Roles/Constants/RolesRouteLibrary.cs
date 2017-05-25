using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class RolesRouteLibrary : ModuleRouteLibrary
    {
        public const string RoutePrefix = "roles";

        #region constructor

        public RolesRouteLibrary(
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

        public string List()
        {
            return AngularRoute();
        }

        public string New()
        {
            return AngularRoute("new");
        }

        public string Details(int id)
        {
            return AngularRoute(id.ToString());
        }

        #endregion
    }
}