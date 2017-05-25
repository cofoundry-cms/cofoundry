using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// Sime bits n bobs to help cut down on boilerplate code when setting up bundling for modules.
    /// </summary>
    public static class BundlingExtensions
    {
        /// <summary>
        /// Adds the main bundle of angular scripts for this module to the bundle collection
        /// </summary>
        /// <param name="jsRoutes">The ModuleJsRouteLibrary for this module</param>
        /// <param name="directories">A collection of directory names (no slashes) to scan for files and add to the bundle.</param>
        /// <returns>BundleCollection instance for chaining</returns>
        public static BundleCollection AddMainAngularScriptBundle(this BundleCollection bundles, ModuleJsRouteLibrary jsRoutes, params string[] directories)
        {
            return bundles.AddMainAngularScriptBundle(jsRoutes, null, directories);
        }

        /// <summary>
        /// Adds the main bundle of angular scripts for this module to the bundle collection
        /// </summary>
        /// <param name="jsRoutes">The ModuleJsRouteLibrary for this module</param>
        /// <param name="directories">A collection of directory names (no slashes) to scan for files and addto the bundle.</param>
        /// <param name="files">A collection of file names to add to the bundle as single resources. This is done before adding directory bundles.</param>
        /// <returns>BundleCollection instance for chaining</returns>
        public static BundleCollection AddMainAngularScriptBundle(this BundleCollection bundles, ModuleJsRouteLibrary jsRoutes, string[] files, string[] directories)
        {
            var bundle = new ScriptBundle(jsRoutes.Main);

            if (files != null)
            {
                foreach (var file in files)
                {
                    bundle.Include(jsRoutes.JsFolderFile(file));
                }
            }

            if (directories != null)
            {
                foreach (var directory in directories)
                {
                    var path = jsRoutes.JsFolderFile(directory + "/");
                    if (HostingEnvironment.VirtualPathProvider.DirectoryExists(path))
                    {
                        bundle.IncludeDirectory(path, "*.js", true);
                    }
                }
            }

            bundles.Add(bundle);

            return bundles;
        }

        /// <summary>
        /// Adds a bundle  to the collection containing angular html template files.
        /// </summary>
        /// <param name="jsRoutes">The ModuleJsRouteLibrary for this module</param>
        /// <param name="directories">A collection of directory names (no slashes) to scan for files and addto the bundle.</param>
        /// <returns>BundleCollection instance for chaining</returns>
        public static BundleCollection AddAngularTemplateBundle(this BundleCollection bundles, ModuleJsRouteLibrary jsRoutes, params string[] directories)
        {
            var bundle = new AngularTemplateBundle(jsRoutes.AngularModuleName, jsRoutes.Templates);

            foreach (var directory in directories)
            {
                var path = jsRoutes.JsFolderFile(directory + "/");
                if (HostingEnvironment.VirtualPathProvider.DirectoryExists(path))
                {
                    bundle.IncludeDirectory(path, "*.html", true);
                }
            }

            bundles.Add(bundle);

            return bundles;
        }
    }
}