using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Optimization;
using Cofoundry.Web.ModularMvc;

namespace Cofoundry.Web.Admin
{
    public class AuthBundleRegistration : IBundleRegistration
    {
        public void RegisterBundles(BundleCollection bundles)
        {
            // JS
            RegisterSingleScriptBundle(bundles, AuthRouteLibrary.Js.ForgotPassword);
            RegisterSingleScriptBundle(bundles, AuthRouteLibrary.Js.Login);
            RegisterSingleScriptBundle(bundles, AuthRouteLibrary.Js.ChangePassword);
        }

        private void RegisterSingleScriptBundle(BundleCollection bundles, string bundlePath)
        {
            var scriptName = Path.GetFileName(bundlePath);
            bundles.Add(new ScriptBundle(bundlePath)
                    .Include(AuthRouteLibrary.Js.MvcViewFolderFile(scriptName + ".js"))
                );
        }
    }
}