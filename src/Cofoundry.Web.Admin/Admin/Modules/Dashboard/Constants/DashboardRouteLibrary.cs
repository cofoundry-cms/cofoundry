using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Admin
{
    public class DashboardRouteLibrary : AngularModuleRouteLibrary
    {
        #region statics

        public const string RoutePrefix = "dashboard";

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
            return AngularRoute();
        }

        #endregion
    }
}