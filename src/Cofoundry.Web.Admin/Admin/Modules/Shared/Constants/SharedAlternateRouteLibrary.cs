using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// Route library for the alternative module (client implementation)
    /// module path.
    /// </summary>
    public class SharedAlternateRouteLibrary : AngularModuleRouteLibrary
    {
        #region constructor

        public SharedAlternateRouteLibrary()
            : base(SharedRouteLibrary.RoutePrefix, RouteConstants.ModuleResourcePathPrefix)
        {
        }

        #endregion
    }
}