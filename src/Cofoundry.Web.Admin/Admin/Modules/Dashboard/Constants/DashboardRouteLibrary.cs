using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class DashboardRouteLibrary : ModuleRouteLibrary
    {
        #region statics

        public const string RoutePrefix = "dashboard";

        public static readonly DashboardRouteLibrary Urls = new DashboardRouteLibrary();

        public static readonly ModuleJsRouteLibrary Js = new ModuleJsRouteLibrary(Urls);

        #endregion

        #region constructor

        public DashboardRouteLibrary()
            : base(RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
        {
        }

        #endregion

        #region routes

        public string Dashboard()
        {
            return CreateAngularRoute();
        }

        #endregion
    }
}