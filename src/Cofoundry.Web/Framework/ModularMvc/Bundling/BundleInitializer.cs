using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Optimization;

namespace Cofoundry.Web.ModularMvc
{
    /// <summary>
    /// Deals with initialization of script/style bundles
    /// </summary>
    public class BundleInitializer : IBundleInitializer
    {
        private readonly IBundleRegistration[] _bundleRegistrations;
        private readonly OptimizationSettings _optimizationSettings;

        public BundleInitializer(
            IBundleRegistration[] bundleRegistrations,
            OptimizationSettings optimizationSettings
            )
        {
            _bundleRegistrations = bundleRegistrations;
            _optimizationSettings = optimizationSettings;
        }

        /// <summary>
        /// Creates a collection of script/style bundles and adds
        /// them to a bundle collection.
        /// </summary>
        public void Initialize()
        {
            foreach (var bundleRegisteration in _bundleRegistrations)
            {
                bundleRegisteration.RegisterBundles(BundleTable.Bundles);
            }

            if (_optimizationSettings.ForceBundling)
            {
                BundleTable.EnableOptimizations = true;
            }
        }
    }
}
