using Cofoundry.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// Contains the commonly used Js routes for modules. You can inherit from this 
    /// and extend it if you have more custom Js routes to add.
    /// </summary>
    public class ModuleJsRouteLibrary
    {
        #region constructor

        private readonly ModuleRouteLibrary _moduleRouteLibrary;

        public ModuleJsRouteLibrary(
            ModuleRouteLibrary moduleRouteLibrary
            )
        {
            _moduleRouteLibrary = moduleRouteLibrary;
            AngularModuleIdentifier = TextFormatter.Camelize(moduleRouteLibrary.ModuleFolderName);
            AngularModuleName = "cms." + AngularModuleIdentifier;
            JsFolderPath = _moduleRouteLibrary.ResourcePrefix + "js";
            MvcViewFolderPath = _moduleRouteLibrary.ResourcePrefix + "mvc/views";
        }

        #endregion

        #region publics

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
        /// Route formatted for registering js files as embedded resources.
        /// </summary>
        public string JsFolderPath { get; private set; }

        public string MvcViewFolderPath { get; private set; }

        /// <summary>
        /// The main js bundle containing all the essential module js files.
        /// </summary>
        public string Main
        {
            get
            {
                return Bundle("main");
            }
        }

        /// <summary>
        /// Bundle containing all the html template files.
        /// </summary>
        public string Templates
        {
            get
            {
                return Bundle("templates");
            }
        }

        public string Bundle(string path)
        {
            return "~" + JsFolderPath + "/" + path;
        }

        public string JsFolderFile(string path)
        {
            return "~" + JsFolderPath + "/" + path;
        }

        /// <summary>
        /// Gets the path of a js file matched with a view (i.e. in the /mvc/views/ folder)
        /// </summary>
        /// <param name="path">file name or path of the js file in the views folder.</param>
        public string MvcViewFolderFile(string path)
        {
            return "~" + MvcViewFolderPath + "/" + path;
        }

        #endregion
    }
}