using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Used to discover additional assemblies to add to the ApplicationPartManager
    /// as candidates for features and DI type discovery. This is done to make features
    /// and DI types accessible from plugins.
    /// </summary>
    public interface IAssemblyDiscoveryProvider
    {
        /// <summary>
        /// Run the discovery routine using the provided ruleset.
        /// </summary>
        /// <param name="mvcBuilder">The application IMvcBuilder configuration.</param>
        /// <param name="rules">The ruleset to use when discovering assemblies.</param>
        /// <returns>
        /// A collection of additional assemblies to reference that aren't already referenced 
        /// by the IMvcBuilder AssemblyPartsProvider.
        /// </returns>
        IEnumerable<Assembly> DiscoverAssemblies(
            IMvcBuilder mvcBuilder,
            IEnumerable<IAssemblyDiscoveryRule> rules
            );
    }
}
