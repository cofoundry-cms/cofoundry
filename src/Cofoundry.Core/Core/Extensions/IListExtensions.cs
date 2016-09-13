using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Core
{
    /// <summary>
    /// Extension methods for classes inherting from IList
    /// </summary>
    public static class IListExtensions
    {
        private static readonly Random globalRandom = new Random();

        /// <summary>
        /// Returns a random item from a list.
        /// </summary>
        /// <typeparam name="T">Type of items to get.</typeparam>
        /// <param name="list">the list to get the random items from.</param>
        /// <param name="random">A random seed generator to use when generating the random item. Can be 
        /// used to make the returned item predictably random.</param>
        /// <returns>A randomly selected item from the list.</returns>
        public static T Random<T>(this IList<T> list, Random random = null)
        {
            random = random ?? globalRandom;
            if (list.Count == 0)
            {
                return default(T);
            }
            return list[random.Next(0, list.Count)];
        }

        /// <summary>
        /// Returns a random item from a list with the possibility of returning the default value.
        /// </summary>
        /// <typeparam name="T">Type of items to get.</typeparam>
        /// <param name="list">the list to get the random items from.</param>
        /// <param name="random">A random seed generator to use when generating the random item. Can be 
        /// used to make the returned item predictably random.</param>
        /// <returns>A randomly selected item from the list or the default value.</returns>
        public static T RandomOrDefault<T>(this IList<T> list, Random random = null)
        {
            random = random ?? globalRandom;
            if (list.Count == 0)
            {
                return default(T);
            }
            var max = list.Count;
            var rnd = random.Next(0, max + 1);
            return rnd == max ? default(T) : list[rnd];
        }

        /// <summary>
        /// Returns a set number of unique random items from the list. 
        /// </summary>
        /// <typeparam name="T">Type of items to get.</typeparam>
        /// <param name="list">the list to get the random items from.</param>
        /// <param name="maxCount">The number of items to return. If there are not enough items in the 
        /// list to meet the maxCount, only the available items will be returned.</param>
        /// <param name="random">A random seed generator to use when generating the random items. Can be 
        /// used to make the returned items predictably random.</param>
        /// <returns>A set number of unique random items from the list.</returns>
        public static IEnumerable<T> Random<T>(this IList<T> list, int maxCount, Random random = null)
        {
            if (maxCount < 1) return Enumerable.Empty<T>();

            random = random ?? globalRandom;
            var randomSortTable = new Dictionary<double, T>();

            foreach(var item in list)
            {
                randomSortTable[random.NextDouble()] = item;
            }

            return randomSortTable
                .OrderBy(KVP => KVP.Key)
                .Take(maxCount)
                .Select(KVP => KVP.Value);
        }

        /// <summary>
        /// Returns all elements of the list in a random order. 
        /// </summary>
        /// <typeparam name="T">Type of list items.</typeparam>
        /// <param name="list">the list to get the random items from.</param>
        /// <param name="random">A random seed generator to use when generating the random items. Can be 
        /// used to make the returned items predictably random.</param>
        /// <returns>collection of items from the list in a random order.</returns>
        public static IEnumerable<T> Randomize<T>(this IList<T> list, Random random = null)
        {
            return list.Random(list.Count, random);
        }

        /// <summary>
        /// Swaps two items in the list
        /// </summary>
        /// <typeparam name="T">Type of item to swap</typeparam>
        /// <param name="list">The lsit to swap items in</param>
        /// <param name="indexA">Index of the first item to swap</param>
        /// <param name="indexB">Index of the second item to swap</param>
        /// <returns></returns>
        public static IList<T> Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
            return list;
        }

        /// <summary>
        /// Adds the value to the list if it is not null, empty or whitespace.
        /// </summary>
        /// <param name="list">List to add the whitespace to</param>
        /// <param name="value">The value to add to the list</param>
        public static void AddIfNotEmpty(this IList<string> list, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                list.Add(value);
            }
        }

        /// <summary>
        /// Filters out empty/null/whitespace values from the collection and removed duplicates.
        /// </summary>
        /// <param name="list">List to filter</param>
        /// <returns>Filtered collection of strings.</returns>
        public static IEnumerable<string> RemoveEmptyAndDuplicates(this IEnumerable<string> list)
        {
            return list.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct();
        }
    }
}
