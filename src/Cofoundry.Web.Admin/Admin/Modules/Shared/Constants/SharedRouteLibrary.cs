using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Admin
{
    public class SharedRouteLibrary : AngularModuleRouteLibrary
    {
        #region constructor

        public const string RoutePrefix = "shared";

        public SharedRouteLibrary()
            : base(RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
        {
        }

        #endregion
    }
}