using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class SetupRouteLibrary : AngularModuleRouteLibrary
    {
        #region statics

        public const string RoutePrefix = "setup";

        public const string SetupLayoutPath = "~/Admin/Modules/Setup/MVC/Views/_SetupLayout.cshtml";

        #endregion

        #region constructor

        public SetupRouteLibrary()
            : base(RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
        {
        }

        #endregion

        #region routes

        public string Setup()
        {
            return MvcRoute();
        }

        #endregion
    }
}