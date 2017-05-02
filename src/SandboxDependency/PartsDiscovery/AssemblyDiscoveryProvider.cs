using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SandboxDependency
{
    public class AssemblyDiscoveryProvider
    {
        public IEnumerable<Assembly> DiscoverAssemblies(
            IMvcBuilder mvcBuilder, 
            IEnumerable<IAssemblyDiscoveryRule> rules
            )
        {
            var environment = GetServiceFromCollection<IHostingEnvironment>(mvcBuilder.Services);
            if (string.IsNullOrEmpty(environment?.ApplicationName)) return Enumerable.Empty<Assembly>();

            var entryAssembly = Assembly.Load(new AssemblyName(environment.ApplicationName));
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

        private static T GetServiceFromCollection<T>(IServiceCollection services)
        {
            return (T)services
                .LastOrDefault(d => d.ServiceType == typeof(T))
                ?.ImplementationInstance;
        }
    }
}
