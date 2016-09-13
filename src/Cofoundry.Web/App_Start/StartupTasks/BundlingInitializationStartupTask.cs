using Cofoundry.Web.ModularMvc;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Cofoundry.Web
{
    /// <summary>
    /// Registers all css/js bundles with the bundling framework.
    /// </summary>
    public class BundlingInitializationStartupTask : IStartupTask
    {
        #region constructor

        private readonly IBundleInitializer _bundleInitializer;

        public BundlingInitializationStartupTask(
            IBundleInitializer bundleInitializer
            )
        {
            _bundleInitializer = bundleInitializer;
        }

        #endregion

        public int Ordering
        {
            get { return (int)StartupTaskOrdering.Normal; }
        }

        public void Run(IAppBuilder app)
        {
            _bundleInitializer.Initialize();
        }
    }
}