using Cofoundry.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// A base class for module url/content routing information.
    /// </summary>
    public class ModuleRouteLibrary
    {
        #region constructor

        /// <summary>
        /// Constructor when you want to use the default resource path (i.e. your
        /// module is located in /cofoundry/admin/modules/{modulename}
        /// </summary>
        /// <param name="routePrefix">
        /// The unique entity/folder name of your route to use when building 
        /// urls e.g. products, users, honeybagders
        /// </param>
        public ModuleRouteLibrary(string routePrefix)
            : this(routePrefix, RouteConstants.ModuleResourcePathPrefix)
        {
        }

        /// <summary>
        /// Constructor to use when you want to provide a custom resource path prefix, for
        /// when your module is located in a non-standard directory (or for all internal 
        /// modules where you pass RouteConstants.InternalModuleResourcePathPrefix)
        /// </summary>
        /// <param name="routePrefix">
        /// The unique entity/folder name of your route to use when building 
        /// urls e.g. products, users, honeybagders
        /// </param>
        /// <param name="resourcePathPrefix">The path prefix to your module e.g. '/admin/modules/'</param>
        public ModuleRouteLibrary(string routePrefix, string resourcePathPrefix)
        {
            ModuleFolderName = TextFormatter.Pascalize(routePrefix);
            ResourcePrefix = resourcePathPrefix + ModuleFolderName.ToLowerInvariant() + "/";
            UrlPrefix = RouteConstants.AdminAreaPrefix + "/" + routePrefix;
        }

        #endregion

        #region properties

        /// <summary>
        /// The name of the folder that contains the module (typically the entity name). Used
        /// for formatting routes.
        /// </summary>
        public string ModuleFolderName { get; private set; }

        /// <summary>
        /// For constructing resource bundle urls i.e. "admin/modules/entity/"
        /// </summary>
        /// <remarks>
        /// Note that resource paths have to mimic the directory structure
        /// because it is based on the embedded resource name.
        /// </remarks>
        public string ResourcePrefix { get; private set; }

        /// <summary>
        /// Prefix for constructing navigation urls i.e. "/admin/entity"
        /// </summary>
        public string UrlPrefix { get; private set; }

        #endregion

        #region helpers

        public string CreateAngularRoute(string path = null)
        {
            return "/" + UrlPrefix + "#/" + path;
        }

        public string CreateMvcRoute(string action = null, string qs = null)
        {
            if (action == null) return "/" + UrlPrefix + qs;

            return "/" + UrlPrefix + "/" + action + qs;
        }

        #endregion
    }
}