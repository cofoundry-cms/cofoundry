using System.Collections.Generic;

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
        /// Returns items in the dictionary which has a key listed in the keysToFilter collection
        /// </summary>
        /// <param name="source">The dictionary to filter</param>
        /// <param name="keysToFilter">Keys to lookup values for</param>
        public static IEnumerable<TValue> FilterByKeyRange<TKey, TValue>(this IDictionary<TKey, TValue> source, IEnumerable<TKey> keysToFilter)
        {
            // Basically does the same thing as ToFilteredAndOrderedCollection but the method name
            // doesn't indicate it so had to create another method
            return source.FilterAndOrderByKeys(keysToFilter);
        }

        /// <summary>
        /// Returns items in the dictionary which has a key listed in the 
        /// orderedKeys collection, in the order they appear in that collection
        /// </summary>
        /// <param name="source">The dictionary to filter</param>
        /// <param name="keysToFilter">Ordered keys to lookup values for</param>
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
