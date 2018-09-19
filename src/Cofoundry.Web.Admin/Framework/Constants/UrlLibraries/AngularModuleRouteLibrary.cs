using Cofoundry.Core;
using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// A base class for module url/content routing information.
    /// </summary>
    public class AngularModuleRouteLibrary : ModuleRouteLibrary
    {
        #region constructor

        /// <summary>
        /// Constructor when you want to use the default resource path (i.e. your
        /// module is located in /cofoundry/admin/modules/{modulename}.
        /// </summary>
        /// <param name="routePrefix">
        /// The unique entity/folder name of your route to use when building 
        /// urls e.g. products, users, honeybagders.
        /// </param>
        public AngularModuleRouteLibrary(
            AdminSettings adminSettings,
            string routePrefix
            )
            : base(adminSettings, routePrefix, RouteConstants.ModuleResourcePathPrefix)
        {
            Angular = new AngularScriptRoutes(this);
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
        public AngularModuleRouteLibrary(
            AdminSettings adminSettings,
            string routePrefix, 
            string resourcePathPrefix
            )
            : base(adminSettings, routePrefix, resourcePathPrefix)
        {
            Angular = new AngularScriptRoutes(this);
        }

        #endregion

        /// <summary>
        /// Script resources for modules using the built-in angular defaults.
        /// </summary>
        public AngularScriptRoutes Angular { get; private set; }
    }
}