using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using Cofoundry.Web.ModularMvc;

namespace Cofoundry.Web.Admin
{
    public class SetupBundleRegistration : IBundleRegistration
    {
        public void RegisterBundles(BundleCollection bundles)
        {
            bundles.AddMainAngularScriptBundle(SetupRouteLibrary.Js,
                AngularJsDirectoryLibrary.Bootstrap,
                AngularJsDirectoryLibrary.Routes,
                AngularJsDirectoryLibrary.DataServices);

            bundles.AddAngularTemplateBundle(SetupRouteLibrary.Js,
                AngularJsDirectoryLibrary.Routes);
        }
    }
}