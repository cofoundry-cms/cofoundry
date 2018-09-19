using Cofoundry.Domain;
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
        public SharedPluginRouteLibrary(AdminSettings adminSettings)
            : base(adminSettings, SharedRouteLibrary.RoutePrefix, RouteConstants.PluginModuleResourcePathPrefix)
        {
        }
    }
}