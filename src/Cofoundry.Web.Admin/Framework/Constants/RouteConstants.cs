using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Admin
{
    public static class RouteConstants
    {
        /// <summary>
        /// Internal name fo the CMS admin area
        /// </summary>
        public const string AdminAreaName = "CofoundryAdmin";

        /// <summary>
        /// For constructing urls i.e. "admin/modules/"
        /// </summary>
        /// <remarks>
        /// Note that resource paths have to mimic the directory structure
        /// because it is based on the embedded resource name.
        /// </remarks>
        public const string ModuleResourcePathPrefix = "/Cofoundry/Admin/Modules/";

        /// <summary>
        /// Similar to ModuleResourcePathPrefix, but for internal modules which will use a 
        /// different path to prevent name clashes
        /// </summary>
        internal const string InternalModuleResourcePathPrefix = "/Admin/Modules/";

        /// <summary>
        /// Similar to ModuleResourcePathPrefix, but for plugin modules which will use a 
        /// different path to prevent name clashes
        /// </summary>
        public const string PluginModuleResourcePathPrefix = "/Plugins/Admin/Modules/";
    }
}