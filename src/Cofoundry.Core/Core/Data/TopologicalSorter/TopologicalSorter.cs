using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Core
{
    /// <summary>
    /// Performs a topological sort, ordering items based on their dependencies.
    /// </summary>
    /// <remarks>
    /// Adapted from https://stackoverflow.com/a/11027096/716689
    /// For more info on topological sorting see https://en.wikipedia.org/wiki/Topological_sorting
    /// </remarks>
    public static class TopologicalSorter
    {
        /// <summary>
        /// Performs a topological sort, ordering items based on their dependencies.
        /// For any ties or items without dependencies the initial ordering is used 
        /// so you may want to sort prior to passing in the collection.
        /// </summary>
        /// <typeparam name="TItem">The type of item being sorted.</typeparam>
        /// <param name="source">
        /// The collection of items to sort. For any ties or items without dependencies
        /// the initial ordering is used so you may want to sort prior to passing in 
        /// the collection.
        /// </param>
        /// <param name="dependencySelector">
        /// A selector used to query dependencies for a particular item. The
        /// parameters are the item to find dependencies for and the source
        /// collection of items to search through.
        /// </param>
        /// <param name="throwOnCyclicDependency">
        /// Set to true if the sorter should throw a CyclicDependencyException when
        /// an item depends on itself of another item which in turn depends on
        /// it. If set to false the cyclic dependency will be ignored and the sorter
        /// will stop processing the dependency chain on the offending item.
        /// </param>
        /// <returns>
        /// Returns the sorted collection. For any ties or items without dependencies
        /// the initial ordering is used.
        /// </returns>
        public static ICollection<TItem> Sort<TItem>(
            ICollection<TItem> source,
            Func<TItem, IEnumerable<TItem>, IEnumerable<TItem>> dependencySelector,
            bool throwOnCyclicDependency
            )
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (dependencySelector == null) throw new ArgumentNullException(nameof(dependencySelector));

            var sorted = new List<TItem>();
            var visited = new HashSet<TItem>();

            foreach (var item in source)
            {
                Visit(item, visited, sorted, source, dependencySelector, throwOnCyclicDependency);
            }

            return sorted;
        }

        private static void Visit<TItem>(
            TItem item, 
            HashSet<TItem> visited, 
            List<TItem> sorted,
            ICollection<TItem> source,
            Func<TItem, IEnumerable<TItem>, IEnumerable<TItem>> dependencySelector,
            bool throwOnCyclicDependency
            )
        {
            if (!visited.Contains(item))
            {
                visited.Add(item);

                foreach (var dep in dependencySelector(item, source))
                {
                    Visit(dep, visited, sorted, source, dependencySelector, throwOnCyclicDependency);
                }

                sorted.Add(item);
            }
            else
            {
                if (throwOnCyclicDependency && !sorted.Contains(item))
                {
                    var msg = $"A cyclic dependency has been detected. The sorted collection already contains the item '{item?.ToString()}' ";
                    throw new CyclicDependencyException(msg);
                }
            }
        }
    }
}
