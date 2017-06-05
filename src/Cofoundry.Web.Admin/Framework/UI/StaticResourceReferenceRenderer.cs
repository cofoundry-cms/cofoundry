using Cofoundry.Core;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// Used to make rendering urls and tags for js and css files easier. Urls
    /// are automatically versioned using IStaticFilePathFormatter and 
    /// DebugSettings.UseUncompressedResources is used to allow unminified
    /// resources to be referenced for debugging.
    /// </summary>
    public class StaticResourceReferenceRenderer : IStaticResourceReferenceRenderer
    {
        private readonly IStaticFilePathFormatter _staticFilePathFormatter;
        private readonly IStaticResourceFileProvider _staticResourceFileProvider;
        private readonly DebugSettings _debugSettings;

        public StaticResourceReferenceRenderer(
            IStaticFilePathFormatter staticFilePathFormatter,
            IStaticResourceFileProvider staticResourceFileProvider,
            DebugSettings debugSettings
            )
        {
            _staticFilePathFormatter = staticFilePathFormatter;
            _staticResourceFileProvider = staticResourceFileProvider;
            _debugSettings = debugSettings;
        }

        /// <summary>
        /// Returns an application relative path to a js file in the conventional 
        /// '[modulepath]/Content/js' directory. The path is automatically versioned e.g. 'myfile.js?v=uniquehash. 
        /// The renderer first checks for a minified file by appending '_min' to the filename and
        /// will use that file unless DebugSettings.UseUncompressedResources is set to true.
        /// </summary>
        /// <param name="moduleRouteLibrary">Route library for the module to render the script for.</param>
        /// <param name="fileName">The javascript filename without a .js extension.</param>
        public string JsPath(ModuleRouteLibrary moduleRouteLibrary, string fileName)
        {
            fileName = Path.GetFileNameWithoutExtension(fileName);
            string virtualPath = null;

            // check for a minified resource first
            if (!_debugSettings.UseUncompressedResources)
            {
                var minPath = moduleRouteLibrary.JsFile(fileName + "_min");

                if (FileExists(minPath))
                {
                    virtualPath = minPath;
                }
            }

            if (virtualPath == null)
            {
                virtualPath = moduleRouteLibrary.JsFile(fileName);
            }

            return _staticFilePathFormatter.AppendVersion(virtualPath);
        }

        /// <summary>
        /// Returns an application relative path to a css file in the conventional 
        /// '[modulepath]/Content/css' directory. The path is automatically versioned e.g. 'myfile.css?v=uniquehash. 
        /// The renderer first checks for a minified file by appending '_min' to the filename and
        /// will use that file unless DebugSettings.UseUncompressedResources is set to true.
        /// </summary>
        /// <param name="moduleRouteLibrary">Route library for the module to render the stylesheet for.</param>
        /// <param name="fileName">The stylesheet filename without a .css extension.</param>
        public string CssPath(ModuleRouteLibrary moduleRouteLibrary, string fileName)
        {
            fileName = Path.GetFileNameWithoutExtension(fileName);
            string virtualPath = moduleRouteLibrary.CssFile(fileName);

            return _staticFilePathFormatter.AppendVersion(virtualPath);
        }

        /// <summary>
        /// Returns a script tag with an application relative path to a js file in the conventional 
        /// '[modulepath]/Content/js' directory. The path is automatically versioned e.g. 'myfile.js?v=uniquehash. 
        /// The renderer first checks for a minified file by appending '_min' to the filename and
        /// will use that file unless DebugSettings.UseUncompressedResources is set to true.
        /// </summary>
        /// <param name="moduleRouteLibrary">Route library for the module to render the script for.</param>
        /// <param name="fileName">The javascript filename without a .js extension.</param>
        public HtmlString ScriptTag(ModuleRouteLibrary moduleRouteLibrary, string fileName)
        {
            var jsPath = JsPath(moduleRouteLibrary, fileName);

            return FormatScriptTag(jsPath);
        }

        /// <summary>
        /// Returns a script tag with an application relative path to a js file in the conventional 
        /// '[modulepath]/Content/js' directory if it exists, otherwise an empty HtmlString is returned. 
        /// The path is automatically versioned e.g. 'myfile.js?v=uniquehash. The renderer first checks 
        /// for a minified file by appending '_min' to the filename and will use that file unless 
        /// DebugSettings.UseUncompressedResources is set to true.
        /// </summary>
        /// <param name="moduleRouteLibrary">Route library for the module to render the script for.</param>
        /// <param name="fileName">The javascript filename without a .js extension.</param>
        public HtmlString ScriptTagIfExists(ModuleRouteLibrary moduleRouteLibrary, string fileName)
        {
            var jsPath = JsPath(moduleRouteLibrary, fileName);

            if (!FileExists(jsPath)) return HtmlString.Empty;

            return FormatScriptTag(jsPath);
        }

        /// <summary>
        /// Returns a link tag containing an application relative path to a css file in the conventional 
        /// '[modulepath]/Content/css' directory. The path is automatically versioned e.g. 'myfile.css?v=uniquehash. 
        /// The renderer first checks for a minified file by appending '_min' to the filename and
        /// will use that file unless DebugSettings.UseUncompressedResources is set to true.
        /// </summary>
        /// <param name="moduleRouteLibrary"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public HtmlString CssTag(ModuleRouteLibrary moduleRouteLibrary, string fileName)
        {
            var cssPath = CssPath(moduleRouteLibrary, fileName);

            return FormatCssTag(cssPath);
        }

        /// <summary>
        /// Returns a link tag containing an application relative path to a css file in the conventional 
        /// '[modulepath]/Content/css' directory if it exists, otherwise an empty HtmlString is returned. 
        /// The path is automatically versioned e.g. 'myfile.css?v=uniquehash. The renderer first checks 
        /// for a minified file by appending '_min' to the filename and will use that file unless 
        /// DebugSettings.UseUncompressedResources is set to true.
        /// </summary>
        /// <param name="moduleRouteLibrary">Route library for the module to render the script for.</param>
        /// <param name="fileName">The javascript filename without a .js extension.</param>
        public HtmlString CssTagIfExists(ModuleRouteLibrary moduleRouteLibrary, string fileName)
        {
            var cssPath = CssPath(moduleRouteLibrary, fileName);

            if (!FileExists(cssPath)) return HtmlString.Empty;

            return FormatCssTag(cssPath);
        }

        private HtmlString FormatScriptTag(string jsPath)
        {
            return new HtmlString($"<script src=\"{jsPath}\"></script>");
        }

        private HtmlString FormatCssTag(string cssPath)
        {
            return new HtmlString($"<link href=\"{cssPath}\" rel=\"stylesheet\"></link>");
        }

        private bool FileExists(string virtualPath)
        {
            return _staticResourceFileProvider.GetFileInfo(virtualPath).Exists;
        }
    }
}