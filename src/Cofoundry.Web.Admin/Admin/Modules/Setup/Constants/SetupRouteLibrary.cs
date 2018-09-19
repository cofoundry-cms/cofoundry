using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Admin
{
    public class SetupRouteLibrary : AngularModuleRouteLibrary
    {
        public const string RoutePrefix = "setup";

        public const string SetupLayoutPath = "~/Admin/Modules/Setup/MVC/Views/_SetupLayout.cshtml";

        public SetupRouteLibrary(AdminSettings adminSettings)
            : base(adminSettings, RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
        {
        }

        #region routes

        public string Setup()
        {
            return MvcRoute();
        }

        #endregion
    }
}