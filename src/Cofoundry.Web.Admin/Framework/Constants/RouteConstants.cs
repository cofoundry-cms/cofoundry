using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public static class RouteConstants
    {
        /// <summary>
        /// Internal name fo the CMS admin area
        /// </summary>
        public const string AdminAreaName = "CofoundryAdmin";

        /// <summary>
        /// The name of the naked admin route prefix i.e. 'admin'
        /// </summary>
        public const string AdminAreaPrefix = "admin";

        /// <summary>
        /// For constructing urls i.e. "/admin"
        /// </summary>
        public const string AdminUrlRoot = "/" + AdminAreaPrefix;

        /// <summary>
        /// For constructing urls i.e. "/admin/api"
        /// </summary>
        public const string ApiUrlRoot = AdminUrlRoot + "/api";

        /// <summary>
        /// For constructing api controller routes i.e. "admin/api/"
        /// </summary>
        public const string ApiRoutePrefix = AdminAreaPrefix + "/api";

        /// <summary>
        /// For constructing api controller routes i.e. "admin/api/plugins"
        /// </summary>
        public const string PluginApiRoutePrefix = ApiRoutePrefix + "/plugins";

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