using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Admin
{


    //    private string GetAlternateResourcePath(string path)
    //    {
    //        return path.Replace("~/", "~/Cofoundry/");
    //    }

    //public class SharedBundleRegistration : IBundleRegistration
    //{
    //    public void RegisterBundles(BundleCollection bundles)
    //    {
    //        var urlTransform = new CssRewriteUrlTransform();

    //        // CSS

    //        var cssBundle = new StyleBundle(SharedRouteLibrary.Css.Main)
    //                .Include(SharedRouteLibrary.Css.Bundle("lib/font-awesome/font-awesome.css"), urlTransform)
    //                .Include(SharedRouteLibrary.Css.Bundle("lib/tinymce/skin.min.css"))
    //                .Include(SharedRouteLibrary.Css.Bundle("lib/ui-select.css"))
    //                .Include(SharedRouteLibrary.Css.Bundle("lib/selectize.default.css"))
    //                .Include(SharedRouteLibrary.Css.Bundle("shared.css"), urlTransform);

    //        // Add in 
    //        var cssOverridePath = GetAlternateResourcePath(SharedRouteLibrary.Css.Bundle("/"));
    //        if (HostingEnvironment.VirtualPathProvider.DirectoryExists(cssOverridePath))
    //        {
    //            cssBundle.IncludeDirectory(cssOverridePath, "*.css", true);
    //        }

    //        bundles.Add(cssBundle);

    //        // Scripts

    //        bundles.Add(new ScriptBundle(SharedRouteLibrary.Js.Html5Shiv)
    //            .Include(SharedRouteLibrary.Js.Bundle("lib/html5shiv.js")));

    //        AddMainScriptBundle(bundles);
    //        AddTemplateBundles(bundles);

    //    }

    //    private void AddMainScriptBundle(BundleCollection bundles)
    //    {
    //        var sharedMainBundle = new ScriptBundle(SharedRouteLibrary.Js.Main);

    //        sharedMainBundle.Include(SharedRouteLibrary.Js.Bundle("lib/underscore.min.js"));
    //        sharedMainBundle.Include(SharedRouteLibrary.Js.Bundle("lib/angular.min.js"));
    //        sharedMainBundle.Include(SharedRouteLibrary.Js.Bundle("lib/angular-sanitize.min.js"));
    //        sharedMainBundle.Include(SharedRouteLibrary.Js.Bundle("lib/AngularModules/TinyMce/tinymce.min.js"));
    //        sharedMainBundle.Include(SharedRouteLibrary.Js.Bundle("lib/AngularModules/TinyMce/ui-tinymce.min.js"));

    //        IncludeDirectory(sharedMainBundle, SharedRouteLibrary.Js.Bundle("Lib/AngularModules/"), false);
    //        IncludeDirectory(sharedMainBundle, SharedRouteLibrary.Js.Bundle(AngularJsDirectoryLibrary.Bootstrap + "/"), true);
    //        IncludeDirectory(sharedMainBundle, SharedRouteLibrary.Js.Bundle(AngularJsDirectoryLibrary.DataServices + "/"), true);
    //        IncludeDirectory(sharedMainBundle, SharedRouteLibrary.Js.Bundle("Utilities/"), true);
    //        IncludeDirectory(sharedMainBundle, SharedRouteLibrary.Js.Bundle("Filters/"), true);
    //        IncludeDirectory(sharedMainBundle, SharedRouteLibrary.Js.Bundle("Framework/"), true);
    //        IncludeDirectory(sharedMainBundle, SharedRouteLibrary.Js.Bundle(AngularJsDirectoryLibrary.UIComponents + "/"), true);

    //        // In an implementation shared scripts are nested in a different folder to avoid name clashes, import them here.
    //        AddAlternateJsDirectoryIfExists(sharedMainBundle, "lib");
    //        AddAlternateJsDirectoryIfExists(sharedMainBundle, AngularJsDirectoryLibrary.Bootstrap);
    //        AddAlternateJsDirectoryIfExists(sharedMainBundle, AngularJsDirectoryLibrary.DataServices);
    //        AddAlternateJsDirectoryIfExists(sharedMainBundle, "Utilities");
    //        AddAlternateJsDirectoryIfExists(sharedMainBundle, "Filters");
    //        AddAlternateJsDirectoryIfExists(sharedMainBundle, "Framework");
    //        AddAlternateJsDirectoryIfExists(sharedMainBundle, AngularJsDirectoryLibrary.UIComponents);

    //        bundles.Add(sharedMainBundle);
    //    }

    //    private void IncludeDirectory(ScriptBundle scriptBundle, string path, bool includeSubDirectories)
    //    {
    //        // Having some issues here with empty directories when publishing to azure, so lets try and give some more info
    //        try
    //        {
    //            scriptBundle.IncludeDirectory(path, "*.js", includeSubDirectories);
    //        }
    //        catch (ArgumentException ex)
    //        {
    //            throw new Exception("Error adding bundle path '" + path + "'.", ex);
    //        }
    //    }

    //    private void AddTemplateBundles(BundleCollection bundles)
    //    {
    //        var templateBundle = new AngularTemplateBundle(SharedRouteLibrary.Js.AngularModuleName, SharedRouteLibrary.Js.Templates);
    //        var uiComponentsPath = SharedRouteLibrary.Js.Bundle(AngularJsDirectoryLibrary.UIComponents + "/");
    //        templateBundle.IncludeDirectory(uiComponentsPath, "*.html", true);

    //        var uiComponentsAlternatePath = GetAlternateResourcePath(uiComponentsPath);
    //        if (HostingEnvironment.VirtualPathProvider.DirectoryExists(uiComponentsAlternatePath))
    //        {
    //            templateBundle.IncludeDirectory(uiComponentsAlternatePath, "*.html", true);
    //        }

    //        bundles.Add(templateBundle);
    //    }

    //    private void AddAlternateJsDirectoryIfExists(Bundle bundle, string directoryName)
    //    {
    //        var path = GetAlternateResourcePath(SharedRouteLibrary.Js.JsFolderFile(directoryName + "/"));
    //        if (HostingEnvironment.VirtualPathProvider.DirectoryExists(path))
    //        {
    //            bundle.IncludeDirectory(path, "*.js", true);
    //        }
    //    }

    //    private string GetAlternateResourcePath(string path)
    //    {
    //        return path.Replace("~/", "~/Cofoundry/");
    //    }
    //}
}