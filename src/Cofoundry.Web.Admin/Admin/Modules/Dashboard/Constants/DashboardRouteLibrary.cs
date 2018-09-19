using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Admin
{
    public class DashboardRouteLibrary : AngularModuleRouteLibrary
    {
        public const string RoutePrefix = "dashboard";
        
        public DashboardRouteLibrary(AdminSettings adminSettings)
            : base(adminSettings, RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
        {
        }

        #region routes

        public string Dashboard()
        {
            return AngularRoute();
        }

        #endregion
    }
}