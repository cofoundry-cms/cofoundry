using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Web.ModularMvc;
using System.Web.Optimization;
using Cofoundry.Core.EmbeddedResources;

namespace Cofoundry.Web.Admin
{
    public class AppRegistration : IBundleRegistration, IEmbeddedResourceRouteRegistration
    {
        public void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/scripts/vnext")
                .Include("~/admin/app/bundles/polyfills.bundle.js")
                .Include("~/admin/app/bundles/vendor.bundle.js")
                .Include("~/admin/app/bundles/app.bundle.js")
                );
        }

        public IEnumerable<string> GetEmbeddedResourcePaths()
        {
            yield return "admin/app/bundles/";
        }
    }
}