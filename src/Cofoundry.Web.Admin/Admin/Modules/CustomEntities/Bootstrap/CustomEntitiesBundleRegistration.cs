using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using Cofoundry.Web.ModularMvc;

namespace Cofoundry.Web.Admin
{
    public class CustomEntitiesBundleRegistration : IBundleRegistration
    {
        public void RegisterBundles(BundleCollection bundles)
        {
            // JS
            bundles.AddMainAngularScriptBundle(CustomEntitiesRouteLibrary.Js,
                AngularJsDirectoryLibrary.Bootstrap,
                AngularJsDirectoryLibrary.Routes,
                AngularJsDirectoryLibrary.Filters,
                AngularJsDirectoryLibrary.DataServices,
                AngularJsDirectoryLibrary.UIComponents);

            bundles.AddAngularTemplateBundle(CustomEntitiesRouteLibrary.Js,
                AngularJsDirectoryLibrary.Routes,
                AngularJsDirectoryLibrary.UIComponents);
        }
    }
}