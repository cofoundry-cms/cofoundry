using Cofoundry.Web.ModularMvc;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public void Run(IApplicationBuilder app)
        {
            _bundleInitializer.Initialize();
        }
    }
}