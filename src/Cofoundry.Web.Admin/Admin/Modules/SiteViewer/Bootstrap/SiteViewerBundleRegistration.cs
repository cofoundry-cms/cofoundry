using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Optimization;
using Cofoundry.Web.ModularMvc;

namespace Cofoundry.Web.Admin
{
    public class SiteViewerBundleRegistration : IBundleRegistration
    {
        public void RegisterBundles(BundleCollection bundles)
        {
            // CSS
            bundles.Add(new StyleBundle(SiteViewerRouteLibrary.Css.InnerSiteViewer)
                    .Include(SiteViewerRouteLibrary.Css.Bundle("inner-site-viewer.css"))
                );
            bundles.Add(new StyleBundle(SiteViewerRouteLibrary.Css.OuterSiteViewer)
                    .Include(SiteViewerRouteLibrary.Css.Bundle("site-viewer.css"))
                );

            // JS
            bundles.AddMainAngularScriptBundle(SiteViewerRouteLibrary.Js,
                AngularJsDirectoryLibrary.Bootstrap,
                AngularJsDirectoryLibrary.Routes,
                AngularJsDirectoryLibrary.UIComponents,
                AngularJsDirectoryLibrary.DataServices);

            bundles.AddAngularTemplateBundle(SiteViewerRouteLibrary.Js,
                AngularJsDirectoryLibrary.Routes,
                AngularJsDirectoryLibrary.UIComponents);

            bundles.Add(new ScriptBundle(SiteViewerRouteLibrary.Js.EventAggregator)
                    .Include(SiteViewerRouteLibrary.Js.Bundle("ContentPage/EventAggregator.js"))
                );
        }

    }
}