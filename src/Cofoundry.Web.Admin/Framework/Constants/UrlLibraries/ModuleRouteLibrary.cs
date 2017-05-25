using Cofoundry.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// A base class for module url/content routing information.
    /// </summary>
    public class ModuleRouteLibrary
    {
        private const string CONTENT_FOLDER = "Content";

        #region constructor

        private readonly IStaticResourceFileProvider _staticResourceFileProvider;
        private readonly OptimizationSettings _optimizationSettings;

        /// <summary>
        /// Constructor when you want to use the default resource path (i.e. your
        /// module is located in /cofoundry/admin/modules/{modulename}.
        /// </summary>
        /// <param name="routePrefix">
        /// The unique entity/folder name of your route to use when building 
        /// urls e.g. products, users, honeybagders.
        /// </param>
        public ModuleRouteLibrary(
            string routePrefix, 
            IStaticResourceFileProvider staticResourceFileProvider, 
            OptimizationSettings optimizationSettings
            )
            : this(routePrefix, RouteConstants.ModuleResourcePathPrefix, staticResourceFileProvider, optimizationSettings)
        {
        }

        /// <summary>
        /// Constructor to use when you want to provide a custom resource path prefix, for
        /// when your module is located in a non-standard directory (or for all internal 
        /// modules where you pass RouteConstants.InternalModuleResourcePathPrefix).
        /// </summary>
        /// <param name="routePrefix">
        /// The unique entity/folder name of your route to use when building 
        /// urls e.g. products, users, honeybagders.
        /// </param>
        /// <param name="resourcePathPrefix">The path prefix to your module e.g. '/admin/modules/'</param>
        public ModuleRouteLibrary(
            string routePrefix, 
            string resourcePathPrefix, 
            IStaticResourceFileProvider staticResourceFileProvider, 
            OptimizationSettings optimizationSettings
            )
        {
            _staticResourceFileProvider = staticResourceFileProvider;
            _optimizationSettings = optimizationSettings;

            ModuleFolderName = TextFormatter.Pascalize(routePrefix);
            ResourcePrefix = resourcePathPrefix + ModuleFolderName.ToLowerInvariant() + "/";
            StaticResourcePrefix = FilePathHelper.CombineVirtualPath(ResourcePrefix, CONTENT_FOLDER);
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
        /// Route for the static resource folder i.e. "admin/modules/entity/content"
        /// </summary>
        public string StaticResourcePrefix { get; private set; }

        /// <summary>
        /// Prefix for constructing navigation urls i.e. "/admin/entity"
        /// </summary>
        public string UrlPrefix { get; private set; }

        #endregion

        #region helpers

        public string JsFile(string fileName)
        {
            fileName = Path.GetFileNameWithoutExtension(fileName);

            // check for a minified resource first
            if (!Debugger.IsAttached && !_optimizationSettings.ForceBundling)
            {
                var minPath = FilePathHelper.CombineVirtualPath(StaticResourcePrefix, "js", fileName + "_min.js");

                if (_staticResourceFileProvider.GetFileInfo(minPath).Exists)
                {
                    return minPath;
                }
            }

            return FilePathHelper.CombineVirtualPath(StaticResourcePrefix, "js", fileName + ".js");
        }

        public string CssFile(string fileName)
        {
            return FilePathHelper.CombineVirtualPath(StaticResourcePrefix, "css", fileName + ".css");
        }

        public string AngularRoute(string path = null)
        {
            return "/" + UrlPrefix + "#/" + path;
        }

        public string MvcRoute(string action = null, string qs = null)
        {
            if (action == null) return "/" + UrlPrefix + qs;

            return "/" + UrlPrefix + "/" + action + qs;
        }

        #endregion
    }
}