using Cofoundry.Domain;
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
        public SharedAlternateRouteLibrary(AdminSettings adminSettings)
            : base(adminSettings, SharedRouteLibrary.RoutePrefix, RouteConstants.ModuleResourcePathPrefix)
        {
        }
    }
}