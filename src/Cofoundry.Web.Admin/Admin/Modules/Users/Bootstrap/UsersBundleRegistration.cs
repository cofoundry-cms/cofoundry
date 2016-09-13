using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Optimization;
using Cofoundry.Web.ModularMvc;

namespace Cofoundry.Web.Admin
{
    public class UsersBundleRegistration : IBundleRegistration
    {
        public void RegisterBundles(BundleCollection bundles)
        {
            // JS
            bundles.AddMainAngularScriptBundle(UsersRouteLibrary.Js,
                AngularJsDirectoryLibrary.Bootstrap,
                AngularJsDirectoryLibrary.Routes,
                AngularJsDirectoryLibrary.Filters,
                AngularJsDirectoryLibrary.DataServices,
                AngularJsDirectoryLibrary.UIComponents);

            bundles.AddAngularTemplateBundle(UsersRouteLibrary.Js,
                AngularJsDirectoryLibrary.Routes,
                AngularJsDirectoryLibrary.UIComponents);
        }
    }
}