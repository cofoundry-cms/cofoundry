using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
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
    /// <remarks>
    /// A similar technique is used by the DefaultAssemblyPartDiscoveryProvider in asp.net
    /// core framework, see https://github.com/aspnet/Mvc/blob/c47825944da54e8da781861d1f72b880befdbd87/src/Microsoft.AspNetCore.Mvc.Core/Internal/DefaultAssemblyPartDiscoveryProvider.cs
    /// </remarks>
    public class AssemblyDiscoveryProvider : IAssemblyDiscoveryProvider
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
        public virtual IEnumerable<Assembly> DiscoverAssemblies(
            IMvcBuilder mvcBuilder, 
            IEnumerable<IAssemblyDiscoveryRule> rules
            )
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var dependencyContext = DependencyContext.Load(entryAssembly);
            if (dependencyContext == null) return Enumerable.Empty<Assembly>();

            var existingReferences = mvcBuilder
                .PartManager
                .ApplicationParts
                .OfType<AssemblyPart>()
                .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
            
            var additionalAssemblies = LoadAdditionalApplicationParts(
                entryAssembly,
                dependencyContext, 
                existingReferences,
                rules);

            return additionalAssemblies;
        }

        private static IEnumerable<Assembly> LoadAdditionalApplicationParts(
            Assembly entryAssembly,
            DependencyContext dependencyContext,
            IReadOnlyDictionary<string, AssemblyPart> existingAssemblyParts,
            IEnumerable<IAssemblyDiscoveryRule> rules
            )
        {
            var assemblyPartsToAdd = new Dictionary<string, AssemblyPart>();
            var context = new AssemblyDiscoveryRuleContext(entryAssembly);

            var candidateLibraries = dependencyContext
                .RuntimeLibraries
                .Where(l => !existingAssemblyParts.ContainsKey(l.Name) && rules.Any(r => r.CanInclude(l, context)));

            foreach (var assembly in candidateLibraries
                .SelectMany(l => l.GetDefaultAssemblyNames(dependencyContext))
                .Select(Assembly.Load))
            {
                yield return assembly;
            }
        }
    }
}
