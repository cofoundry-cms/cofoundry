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
    public interface IBundleInitializer
    {
        /// <summary>
        /// Creates a collection of script/style bundles and adds
        /// them to a bundle collection.
        /// </summary>
        void Initialize();
    }
}
