﻿namespace Cofoundry.Core;

/// <summary>
/// Extension methods for classes inherting from IList
/// </summary>
public static class IListExtensions
{
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
        var tmp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = tmp;
        return list;
    }

    /// <summary>
    /// Adds the value to the list if it is not null, empty or whitespace.
    /// </summary>
    /// <param name="list">List to add the whitespace to</param>
    /// <param name="value">The value to add to the list</param>
    public static void AddIfNotEmpty(this IList<string> list, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            list.Add(value);
        }
    }
}
