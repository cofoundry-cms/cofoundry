namespace Cofoundry.Core;

public static class IDictionaryExtensions
{
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
    /// Returns items in the dictionary which has a key listed in the keysToFilter 
    /// collection. The method ensures no duplicates are returned even if they appear
    /// in the keysToFilter collection.
    /// </summary>
    /// <param name="source">The dictionary to filter</param>
    /// <param name="keysToFilter">Keys to lookup values for</param>
    public static IEnumerable<TValue> FilterByKeys<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> source, IEnumerable<TKey> keysToFilter)
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
    public static IEnumerable<TValue> FilterAndOrderByKeys<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> source, IEnumerable<TKey> orderedKeys)
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
