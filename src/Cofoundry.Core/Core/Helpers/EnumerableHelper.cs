using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core
{
    public static class EnumerableHelper
    {
        /// <summary>
        /// Determines if the enumerable is null or contains no elements
        /// </summary>
        public static bool IsNullOrEmpty<T>(IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        /// <summary>
        /// Unions all the specified enumerables, ignoring null collections
        /// </summary>
        public static IEnumerable<T> UnionAll<T>(params IEnumerable<T>[] enumerables)
        {
            if (IsNullOrEmpty(enumerables)) yield break;

            foreach (var enumerable in enumerables)
            {
                if (!IsNullOrEmpty(enumerable))
                {
                    foreach (var item in enumerable)
                    {
                        yield return item;
                    }
                }
            }
        }

        /// <summary>
        /// Unions the specified enumerables, ignoring null collections and returning 
        /// distinct results.
        /// </summary>
        public static IEnumerable<T> Union<T>(params IEnumerable<T>[] enumerables)
        {
            return UnionAll(enumerables).Distinct();
        }

        /// <summary>
        /// Enumerates the specified enumerable, but doesn't throw an 
        /// exception if it is null.
        /// </summary>
        public static IEnumerable<T> Enumerate<T>(IEnumerable<T> enumerable)
        {
            if (enumerable == null) return Enumerable.Empty<T>();

            return enumerable;
        }
    }
}
