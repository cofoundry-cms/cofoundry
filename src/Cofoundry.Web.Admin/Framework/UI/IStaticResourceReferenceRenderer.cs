using Cofoundry.Core;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// Used to make rendering urls and tags for js and css files easier. Urls
    /// are automatically versioned using IStaticFilePathFormatter and 
    /// DebugSettings.UseUncompressedResources is used to allow unminified
    /// resources to be referenced for debugging.
    /// </summary>
    public interface IStaticResourceReferenceRenderer
    {
        /// <summary>
        /// Returns an application relative path to a js file in the conventional 
        /// '[modulepath]/Content/js' directory. The path is automatically versioned e.g. 'myfile.js?v=uniquehash. 
        /// The renderer first checks for a minified file by appending '_min' to the filename and
        /// will use that file unless DebugSettings.UseUncompressedResources is set to true.
        /// </summary>
        /// <param name="moduleRouteLibrary">Route library for the module to render the script for.</param>
        /// <param name="fileName">The javascript filename without a .js extension.</param>
        string JsPath(ModuleRouteLibrary moduleRouteLibrary, string fileName);

        /// <summary>
        /// Returns an application relative path to a css file in the conventional 
        /// '[modulepath]/Content/css' directory. The path is automatically versioned e.g. 'myfile.css?v=uniquehash. 
        /// The renderer first checks for a minified file by appending '_min' to the filename and
        /// will use that file unless DebugSettings.UseUncompressedResources is set to true.
        /// </summary>
        /// <param name="moduleRouteLibrary">Route library for the module to render the stylesheet for.</param>
        /// <param name="fileName">The stylesheet filename without a .css extension.</param>
        string CssPath(ModuleRouteLibrary moduleRouteLibrary, string fileName);

        /// <summary>
        /// Returns a script tag with an application relative path to a js file in the conventional 
        /// '[modulepath]/Content/js' directory. The path is automatically versioned e.g. 'myfile.js?v=uniquehash. 
        /// The renderer first checks for a minified file by appending '_min' to the filename and
        /// will use that file unless DebugSettings.UseUncompressedResources is set to true.
        /// </summary>
        /// <param name="moduleRouteLibrary">Route library for the module to render the script for.</param>
        /// <param name="fileName">The javascript filename without a .js extension.</param>
        HtmlString ScriptTag(ModuleRouteLibrary moduleRouteLibrary, string fileName);

        /// <summary>
        /// Returns a script tag with an application relative path to a js file in the conventional 
        /// '[modulepath]/Content/js' directory if it exists, otherwise an empty HtmlString is returned. 
        /// The path is automatically versioned e.g. 'myfile.js?v=uniquehash. The renderer first checks 
        /// for a minified file by appending '_min' to the filename and will use that file unless 
        /// DebugSettings.UseUncompressedResources is set to true.
        /// </summary>
        /// <param name="moduleRouteLibrary">Route library for the module to render the script for.</param>
        /// <param name="fileName">The javascript filename without a .js extension.</param>
        HtmlString ScriptTagIfExists(ModuleRouteLibrary moduleRouteLibrary, string fileName);

        /// <summary>
        /// Gets a collection of script tags for all the files in the js 
        /// directory of the specified route library.
        /// </summary>
        /// <param name="moduleRouteLibrary">Route library containg the path to find js files in.</param>
        IEnumerable<HtmlString> ScriptTagsForDirectory(ModuleRouteLibrary moduleRouteLibrary);

        /// <summary>
        /// Returns a link tag containing an application relative path to a css file in the conventional 
        /// '[modulepath]/Content/css' directory. The path is automatically versioned e.g. 'myfile.css?v=uniquehash. 
        /// The renderer first checks for a minified file by appending '_min' to the filename and
        /// will use that file unless DebugSettings.UseUncompressedResources is set to true.
        /// </summary>
        /// <param name="moduleRouteLibrary"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        HtmlString CssTag(ModuleRouteLibrary moduleRouteLibrary, string fileName);

        /// <summary>
        /// Returns a link tag containing an application relative path to a css file in the conventional 
        /// '[modulepath]/Content/css' directory if it exists, otherwise an empty HtmlString is returned. 
        /// The path is automatically versioned e.g. 'myfile.css?v=uniquehash. The renderer first checks 
        /// for a minified file by appending '_min' to the filename and will use that file unless 
        /// DebugSettings.UseUncompressedResources is set to true.
        /// </summary>
        /// <param name="moduleRouteLibrary">Route library for the module to render the script for.</param>
        /// <param name="fileName">The javascript filename without a .js extension.</param>
        HtmlString CssTagIfExists(ModuleRouteLibrary moduleRouteLibrary, string fileName);

        /// <summary>
        /// Gets a collection of css tags for all the files in the css 
        /// directory of the specified route library.
        /// </summary>
        /// <param name="moduleRouteLibrary">Route library containg the path to find css files in.</param>
        IEnumerable<HtmlString> CssTagsForDirectory(ModuleRouteLibrary moduleRouteLibrary);
    }
}