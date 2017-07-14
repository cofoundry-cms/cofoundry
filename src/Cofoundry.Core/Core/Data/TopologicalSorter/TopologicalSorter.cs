using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Core
{
    /// <summary>
    /// Performs a topological sort, ordering items based on their dependencies
    /// </summary>
    /// <remarks>
    /// Adapted from https://stackoverflow.com/a/11027096/716689
    /// For more info on topological sorting see https://en.wikipedia.org/wiki/Topological_sorting
    /// </remarks>
    public static class TopologicalSorter
    {
        public static IEnumerable<T> Sort<T>(
            IEnumerable<T> source,
            Func<T, IEnumerable<T>> dependencySelector,
            bool throwOnCyclicDependency
            )
        {
            var sorted = new List<T>();
            var visited = new HashSet<T>();

            foreach (var item in source)
            {
                Visit(item, visited, sorted, dependencySelector, throwOnCyclicDependency);
            }

            return sorted;
        }

        private static void Visit<T>(
            T item, 
            HashSet<T> visited, 
            List<T> sorted, 
            Func<T, IEnumerable<T>> dependencies,
            bool throwOnCyclicDependency
            )
        {
            if (!visited.Contains(item))
            {
                visited.Add(item);

                foreach (var dep in dependencies(item))
                {
                    Visit(dep, visited, sorted, dependencies, throwOnCyclicDependency);
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
