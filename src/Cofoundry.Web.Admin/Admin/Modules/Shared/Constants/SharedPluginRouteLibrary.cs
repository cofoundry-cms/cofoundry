using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// Route library for the plugin module shared code
    /// module path.
    /// </summary>
    public class SharedPluginRouteLibrary : AngularModuleRouteLibrary
    {
        #region constructor

        public SharedPluginRouteLibrary()
            : base(SharedRouteLibrary.RoutePrefix, RouteConstants.PluginModuleResourcePathPrefix)
        {
        }

        #endregion
    }
}