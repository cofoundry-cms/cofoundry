using Cofoundry.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// Script resources for modules using the built-in angular defaults.
    /// </summary>
    public class AngularScriptRoutes
    {
        public AngularScriptRoutes(ModuleRouteLibrary moduleRouteLibrary)
        {
            AngularModuleIdentifier = TextFormatter.Camelize(moduleRouteLibrary.ModuleFolderName);
            AngularModuleName = "cms." + AngularModuleIdentifier;

            var jsPrefix = moduleRouteLibrary.ModuleFolderName.ToLowerInvariant();
            MainScriptName = jsPrefix;
            TemplateScriptName = jsPrefix + "_templates";
        }
            
        /// <summary>
        /// The full name of the module used by angular, e.g. 'cms.myStuff'
        /// </summary>
        public string AngularModuleName { get; private set; }

        /// <summary>
        /// The identifier part of the module name e.g. 'myStuff' that is used to namespace
        /// injectable module components, e.g. 'myStuff.options'
        /// </summary>
        public string AngularModuleIdentifier { get; private set; }

        /// <summary>
        /// Path to the angular module entry script.
        /// </summary>
        public string MainScriptName { get; private set; }

        /// <summary>
        /// Path to the angular template bundle.
        /// </summary>
        public string TemplateScriptName { get; private set; }
    }
}