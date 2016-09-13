using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Optimization;

namespace Cofoundry.Web.ModularMvc
{
    /// <summary>
    /// Allows automatic injection of classes that register style/script bundles.
    /// </summary>
    public interface IBundleRegistration
    {
        void RegisterBundles(BundleCollection bundles);
    }
}
