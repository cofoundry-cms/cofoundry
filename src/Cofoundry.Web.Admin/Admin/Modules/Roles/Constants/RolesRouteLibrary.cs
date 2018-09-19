using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Admin
{
    public class RolesRouteLibrary : ModuleRouteLibrary
    {
        public const string RoutePrefix = "roles";

        public RolesRouteLibrary(AdminSettings adminSettings)
            : base(adminSettings, RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
        {
        }

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