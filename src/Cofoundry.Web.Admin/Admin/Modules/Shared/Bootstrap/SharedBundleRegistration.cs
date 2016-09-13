using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Optimization;
using Cofoundry.Web.ModularMvc;

namespace Cofoundry.Web.Admin
{
    public class SharedBundleRegistration : IBundleRegistration
    {
        public void RegisterBundles(BundleCollection bundles)
        {
            var urlTransform = new CssRewriteUrlTransform();

            // CSS

            var cssBundle = new StyleBundle(SharedRouteLibrary.Css.Main)
                    .Include(SharedRouteLibrary.Css.Bundle("lib/font-awesome/font-awesome.css"), urlTransform)
                    .Include(SharedRouteLibrary.Css.Bundle("lib/textangular.css"))
                    .Include(SharedRouteLibrary.Css.Bundle("lib/ui-select.css"))
                    .Include(SharedRouteLibrary.Css.Bundle("lib/selectize.default.css"))
                    .Include(SharedRouteLibrary.Css.Bundle("shared.css"), urlTransform);

            // Add in 
            var cssOverridePath = GetAlternateResourcePath(SharedRouteLibrary.Css.Bundle("/"));
            if (HostingEnvironment.VirtualPathProvider.DirectoryExists(cssOverridePath))
            {
                cssBundle.IncludeDirectory(cssOverridePath, "*.css", true);
            }

            bundles.Add(cssBundle);

            // Scripts
            
            bundles.Add(new ScriptBundle(SharedRouteLibrary.Js.Html5Shiv)
                .Include(SharedRouteLibrary.Js.Bundle("lib/html5shiv.js")));

            AddMainScriptBundle(bundles);
            AddTemplateBundles(bundles);

        }

        private void AddMainScriptBundle(BundleCollection bundles)
        {
            var sharedMainBundle = new ScriptBundle(SharedRouteLibrary.Js.Main)
               .Include(SharedRouteLibrary.Js.Bundle("lib/underscore.min.js"))
               .Include(SharedRouteLibrary.Js.Bundle("lib/angular.min.js"))
               .Include(SharedRouteLibrary.Js.Bundle("Lib/AngularModules/TextAngular/textAngular-rangy.min.js"))
               .Include(SharedRouteLibrary.Js.Bundle("Lib/AngularModules/TextAngular/textAngular-sanitize.min.js"))
               .Include(SharedRouteLibrary.Js.Bundle("Lib/AngularModules/TextAngular/textAngular.min.js"))
               .IncludeDirectory(SharedRouteLibrary.Js.Bundle("Lib/AngularModules/"), "*.js", false)
               .IncludeDirectory(SharedRouteLibrary.Js.Bundle(AngularJsDirectoryLibrary.Bootstrap + "/"), "*.js", true)
               .IncludeDirectory(SharedRouteLibrary.Js.Bundle(AngularJsDirectoryLibrary.DataServices + "/"), "*.js", true)
               .IncludeDirectory(SharedRouteLibrary.Js.Bundle("utilities/"), "*.js", true)
               .IncludeDirectory(SharedRouteLibrary.Js.Bundle("filters/"), "*.js", true)
               .IncludeDirectory(SharedRouteLibrary.Js.Bundle("framework/"), "*.js", true)
               .IncludeDirectory(SharedRouteLibrary.Js.Bundle(AngularJsDirectoryLibrary.UIComponents + "/"), "*.js", true);

            // In an implementation shared scripts are nested in a different folder to avoid name clashes, import them here.
            AddAlternateJsDirectoryIfExists(sharedMainBundle, "Lib");
            AddAlternateJsDirectoryIfExists(sharedMainBundle, AngularJsDirectoryLibrary.Bootstrap);
            AddAlternateJsDirectoryIfExists(sharedMainBundle, AngularJsDirectoryLibrary.DataServices);
            AddAlternateJsDirectoryIfExists(sharedMainBundle, "utilities");
            AddAlternateJsDirectoryIfExists(sharedMainBundle, "filters");
            AddAlternateJsDirectoryIfExists(sharedMainBundle, "framework");
            AddAlternateJsDirectoryIfExists(sharedMainBundle, AngularJsDirectoryLibrary.UIComponents);

            bundles.Add(sharedMainBundle);
        }

        private void AddTemplateBundles(BundleCollection bundles)
        {
            var templateBundle = new AngularTemplateBundle(SharedRouteLibrary.Js.AngularModuleName, SharedRouteLibrary.Js.Templates);
            var uiComponentsPath = SharedRouteLibrary.Js.Bundle(AngularJsDirectoryLibrary.UIComponents + "/");
            templateBundle.IncludeDirectory(uiComponentsPath, "*.html", true);

            var uiComponentsAlternatePath = GetAlternateResourcePath(uiComponentsPath);
            if (HostingEnvironment.VirtualPathProvider.DirectoryExists(uiComponentsAlternatePath))
            {
                templateBundle.IncludeDirectory(uiComponentsAlternatePath, "*.html", true);
            }

            bundles.Add(templateBundle);
        }

        private void AddAlternateJsDirectoryIfExists(Bundle bundle, string directoryName)
        {
            var path = GetAlternateResourcePath(SharedRouteLibrary.Js.JsFolderFile(directoryName + "/"));
            if (HostingEnvironment.VirtualPathProvider.DirectoryExists(path))
            {
                bundle.IncludeDirectory(path, "*.js", true);
            }
        }

        private string GetAlternateResourcePath(string path)
        {
            return path.Replace("~/", "~/Cofoundry/");
        }
    }
}