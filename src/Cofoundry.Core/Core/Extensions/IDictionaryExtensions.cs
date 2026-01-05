namespace Cofoundry.Core;

/// <summary>
/// Extension methods for <see cref="IDictionary{TKey, TValue}"/> types.
/// </summary>
public static class IDictionaryExtensions
{
    extension<TKey, TValue>(Dictionary<TKey, TValue> source) where TKey : notnull
    {
        /// <summary>
        /// Returns items in the dictionary which has a key listed in the <paramref name="keysToFilter"/> 
        /// collection. The method ensures no duplicates are returned even if they appear in the
        /// <paramref name="keysToFilter"/> collection.
        /// </summary>
        /// <param name="keysToFilter">Keys to lookup values for</param>
        public IEnumerable<TValue> FilterByKeys(IEnumerable<TKey> keysToFilter)
        {
            return source.FilterAndOrderByKeys(keysToFilter.Distinct());
        }

        /// <summary>
        /// Returns items in the dictionary which has a key listed in the <paramref name="orderedKeys"/>
        /// collection, in the order they appear in that collection. Duplicates may be returned if
        /// the ordered keys collections contains them.
        /// </summary>
        /// <param name="orderedKeys">
        /// A collection of dictionary keys in the order that you would like the results
        /// return in. Duplicate items may be returned if the orderedKeys collection 
        /// contains duplicates.
        /// </param>
        public IEnumerable<TValue> FilterAndOrderByKeys(IEnumerable<TKey> orderedKeys)
        {
            if (orderedKeys == null)
            {
                yield break;
            }

            foreach (var key in orderedKeys)
            {
                if (source.TryGetValue(key, out var value))
                {
                    yield return value;
                }
            }
        }
    }

    extension<TKey, TValue>(IDictionary<TKey, TValue> source)
    {
        /// <summary>
        /// Returns items in the dictionary which has a key listed in the <paramref name="keysToFilter"/> 
        /// collection. The method ensures no duplicates are returned even if they appear in the
        /// <paramref name="keysToFilter"/> collection.
        /// </summary>
        /// <param name="keysToFilter">Keys to lookup values for</param>
        public IEnumerable<TValue> FilterByKeys(IEnumerable<TKey> keysToFilter)
        {
            return source.FilterAndOrderByKeys(keysToFilter.Distinct());
        }

        /// <summary>
        /// Returns items in the dictionary which has a key listed in the <paramref name="orderedKeys"/>
        /// collection, in the order they appear in that collection. Duplicates may be returned if
        /// the ordered keys collections contains them.
        /// </summary>
        /// <param name="orderedKeys">
        /// A collection of dictionary keys in the order that you would like the results
        /// return in. Duplicate items may be returned if the orderedKeys collection 
        /// contains duplicates.
        /// </param>
        public IEnumerable<TValue> FilterAndOrderByKeys(IEnumerable<TKey> orderedKeys)
        {
            if (orderedKeys == null)
            {
                yield break;
            }

            foreach (var key in orderedKeys)
            {
                if (source.TryGetValue(key, out var value))
                {
                    yield return value;
                }
            }
        }
    }

    extension<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> source)
    {
        /// <summary>
        /// Returns items in the dictionary which has a key listed in the <paramref name="keysToFilter"/> 
        /// collection. The method ensures no duplicates are returned even if they appear in the
        /// <paramref name="keysToFilter"/> collection.
        /// </summary>
        /// <param name="keysToFilter">Keys to lookup values for</param>
        public IEnumerable<TValue> FilterByKeys(IEnumerable<TKey> keysToFilter)
        {
            return source.FilterAndOrderByKeys(keysToFilter.Distinct());
        }

        /// <summary>
        /// Returns items in the dictionary which has a key listed in the <paramref name="orderedKeys"/>
        /// collection, in the order they appear in that collection. Duplicates may be returned if
        /// the ordered keys collections contains them.
        /// </summary>
        /// <param name="orderedKeys">
        /// A collection of dictionary keys in the order that you would like the results
        /// return in. Duplicate items may be returned if the orderedKeys collection 
        /// contains duplicates.
        /// </param>
        public IEnumerable<TValue> FilterAndOrderByKeys(IEnumerable<TKey> orderedKeys)
        {
            if (orderedKeys == null)
            {
                yield break;
            }

            foreach (var key in orderedKeys)
            {
                if (source.TryGetValue(key, out var value))
                {
                    yield return value;
                }
            }
        }
    }
}
