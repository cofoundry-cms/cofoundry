using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Optimization;
using Cofoundry.Web.ModularMvc;

namespace Cofoundry.Web.Admin
{
    public class VisualEditorBundleRegistration : IBundleRegistration
    {
        public void RegisterBundles(BundleCollection bundles)
        {
            // CSS
            bundles.Add(new StyleBundle(VisualEditorRouteLibrary.Css.VisualEditor)
                    .Include(VisualEditorRouteLibrary.Css.Bundle("site-viewer.css"))
                );

            // JS
            bundles.AddMainAngularScriptBundle(VisualEditorRouteLibrary.Js,
                AngularJsDirectoryLibrary.Bootstrap,
                AngularJsDirectoryLibrary.Routes,
                AngularJsDirectoryLibrary.UIComponents,
                AngularJsDirectoryLibrary.DataServices);

            bundles.AddAngularTemplateBundle(VisualEditorRouteLibrary.Js,
                AngularJsDirectoryLibrary.Routes,
                AngularJsDirectoryLibrary.UIComponents);

            bundles.Add(new ScriptBundle(VisualEditorRouteLibrary.Js.VisualEditor)
                    .Include(VisualEditorRouteLibrary.Js.Bundle("ContentPage/Index.js"))
                    .Include(VisualEditorRouteLibrary.Js.Bundle("ContentPage/GuiController.js"))
                    .Include(VisualEditorRouteLibrary.Js.Bundle("ContentPage/EventAggregator.js"))
                );
        }
    }
}