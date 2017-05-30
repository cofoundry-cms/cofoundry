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
    public class StaticResourceReferenceRenderer : IStaticResourceReferenceRenderer
    {
        private readonly IStaticFilePathFormatter _staticFilePathFormatter;
        private readonly IStaticResourceFileProvider _staticResourceFileProvider;
        private readonly OptimizationSettings _optimizationSettings;

        public StaticResourceReferenceRenderer(
            IStaticFilePathFormatter staticFilePathFormatter,
            IStaticResourceFileProvider staticResourceFileProvider,
            OptimizationSettings optimizationSettings
            )
        {
            _staticFilePathFormatter = staticFilePathFormatter;
            _staticResourceFileProvider = staticResourceFileProvider;
            _optimizationSettings = optimizationSettings;
        }

        public string JsPath(ModuleRouteLibrary moduleRouteLibrary, string fileName)
        {
            fileName = Path.GetFileNameWithoutExtension(fileName);
            string virtualPath = null;

            // check for a minified resource first
            if (!Debugger.IsAttached && !_optimizationSettings.ForceBundling)
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

        public string CssPath(ModuleRouteLibrary moduleRouteLibrary, string fileName)
        {
            fileName = Path.GetFileNameWithoutExtension(fileName);
            string virtualPath = moduleRouteLibrary.CssFile(fileName);

            return _staticFilePathFormatter.AppendVersion(virtualPath);
        }

        public HtmlString ScriptTag(ModuleRouteLibrary moduleRouteLibrary, string fileName)
        {
            var jsPath = JsPath(moduleRouteLibrary, fileName);

            return FormatScriptTag(jsPath);
        }

        public HtmlString ScriptTagIfExists(ModuleRouteLibrary moduleRouteLibrary, string fileName)
        {
            var jsPath = JsPath(moduleRouteLibrary, fileName);

            if (!FileExists(jsPath)) return HtmlString.Empty;

            return FormatScriptTag(jsPath);
        }

        public HtmlString CssTag(ModuleRouteLibrary moduleRouteLibrary, string fileName)
        {
            var cssPath = CssPath(moduleRouteLibrary, fileName);

            return FormatCssTag(cssPath);
        }

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