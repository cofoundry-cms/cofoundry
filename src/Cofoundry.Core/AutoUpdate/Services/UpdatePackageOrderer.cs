using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Core.AutoUpdate.Internal
{
    /// <summary>
    /// Order UpdatePackage instances into the correct execution order, making
    /// sure dependencies are taken into consideration and making sure the Cofoundry
    /// base package always runs first.
    /// </summary>
    public class UpdatePackageOrderer : IUpdatePackageOrderer
    {
        /// <summary>
        /// Orders a collection of UpdatePackage instances into the correct execution 
        /// order, making sure dependencies are taken into consideration.
        /// </summary>
        /// <param name="packages">update packages to sort.</param>
        /// <returns>Enumerable collection of update packages, sorted into the correct order.</returns>
        public ICollection<UpdatePackage> Order(ICollection<UpdatePackage> packages)
        {
            // Build a collection of dependency instances to use in sorting
            var dependentModuleLookup = packages
                .Where(p => !EnumerableHelper.IsNullOrEmpty(p.DependentModules))
                .SelectMany(p => p.DependentModules)
                .Distinct()
                .ToDictionary(k => k, v => packages.Where(p => p.ModuleIdentifier == v));

            // Make sure Cofoundry packages are installed first, even if someone has forgotten 
            // to mark it as a dependency. Use secondary sorting to ensure deterministic ordering.
            var orderedPackages = packages
                .OrderByDescending(p => p.ModuleIdentifier == CofoundryModuleInfo.ModuleIdentifier)
                .ThenBy(p => p.ModuleIdentifier)
                .ThenBy(p => p.GetType().FullName)
                .ToList();

            // Sort based on dependencies
            var topologicallySortedPackages = TopologicalSorter.Sort(orderedPackages, (p, l) => FindDependencies(p, dependentModuleLookup), true);

            return topologicallySortedPackages;
        }

        private IEnumerable<UpdatePackage> FindDependencies(UpdatePackage packageToGetDependenciesFor, Dictionary<string, IEnumerable<UpdatePackage>> dependentModuleLookup)
        {
            var dependencies = EnumerableHelper
                .Enumerate(packageToGetDependenciesFor.DependentModules)
                .SelectMany(moduleCode => dependentModuleLookup.GetOrDefault(moduleCode, Enumerable.Empty<UpdatePackage>()));

            return dependencies;
        }
    }
}
