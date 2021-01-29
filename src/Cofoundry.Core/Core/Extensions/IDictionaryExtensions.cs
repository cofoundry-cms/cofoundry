using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Core
{
    public static class IDictionaryExtensions
    {
        /// <summary>
        /// Gets a value if it exists in the dictionary, otherise returns the default value.
        /// </summary>
        /// <param name="source">The dictionary to act on</param>
        /// <param name="key">Key of the dictionary item to get</param>
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        {
            if (key == null) return default(TValue);
            TValue value;
            return source.TryGetValue(key, out value) ? value : default(TValue);
        }

        /// <summary>
        /// Gets a value if it exists in the dictionary, otherise returns the default value. When
        /// using with nullable types, a null key always returns the default TValue.
        /// </summary>
        /// <param name="source">The dictionary to act on.</param>
        /// <param name="key">Key of the dictionary item to get. If null, then a default TValue is returned without checking the dictionary.</param>
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey? key)
            where TKey : struct
        {
            if (!key.HasValue) return default(TValue);
            TValue value;
            return source.TryGetValue(key.Value, out value) ? value : default(TValue);
        }

        /// <summary>
        /// Gets a value if it exists in the dictionary, otherise returns the specified default value.
        /// </summary>
        /// <param name="source">The dictionary to act on.</param>
        /// <param name="key">Key of the dictionary item to get.</param>
        /// <param name="def">Default value to return if it is missing.</param>
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue def)
        {
            if (key == null) return def;
            TValue value;
            return source.TryGetValue(key, out value) ? value : def;
        }

        /// <summary>
        /// Gets a value if it exists in the dictionary, otherise returns the specified default value. When
        /// using with nullable types, a null key always returns the default value.
        /// </summary>
        /// <param name="source">The dictionary to act on.</param>
        /// <param name="key">Key of the dictionary item to get. If null, then the default value is returned without checking the dictionary.</param>
        /// <param name="def">Default value to return if it is missing.</param>
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey? key, TValue def)
            where TKey : struct
        {
            if (!key.HasValue) return def;
            TValue value;
            return source.TryGetValue(key.Value, out value) ? value : def;
        }

        /// <summary>
        /// Returns items in the dictionary which has a key listed in the keysToFilter 
        /// collection. The method ensures no duplicates are returned even if they appear
        /// in the keysToFilter collection.
        /// </summary>
        /// <param name="source">The dictionary to filter</param>
        /// <param name="keysToFilter">Keys to lookup values for</param>
        public static IEnumerable<TValue> FilterByKeys<TKey, TValue>(this IDictionary<TKey, TValue> source, IEnumerable<TKey> keysToFilter)
        {
            return source.FilterAndOrderByKeys(keysToFilter.Distinct());
        }

        /// <summary>
        /// Returns items in the dictionary which has a key listed in the 
        /// orderedKeys collection, in the order they appear in that collection.
        /// Duplicates may be returned if the ordered keys collections contains 
        /// them.
        /// </summary>
        /// <param name="source">The dictionary to filter</param>
        /// <param name="orderedKeys">
        /// A collection of dictionary keys in the order that you would like the results
        /// return in. Duplicate items may be returned if the orderedKeys collection 
        /// contains duplicates.
        /// </param>
        public static IEnumerable<TValue> FilterAndOrderByKeys<TKey, TValue>(this IDictionary<TKey, TValue> source, IEnumerable<TKey> orderedKeys)
        {
            if (orderedKeys == null) yield break;
            TValue value;

            foreach (var key in orderedKeys)
            {
                if (source.TryGetValue(key, out value))
                {
                    yield return value;
                }
            }
        }
    }
}
