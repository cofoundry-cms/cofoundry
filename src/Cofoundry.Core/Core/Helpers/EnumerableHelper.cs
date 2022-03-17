namespace Cofoundry.Core;

public static class EnumerableHelper
{
    /// <summary>
    /// Determines if the enumerable is <see langword="null"/> or contains 
    /// no elements.
    /// </summary>
    public static bool IsNullOrEmpty<T>(IEnumerable<T> source)
    {
        return source == null || !source.Any();
    }

    /// <summary>
    /// Unions all elements in <paramref name="enumerables"/>, ignoring null collections.
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
    /// Unions the elements in <paramref name="enumerables"/>, ignoring <see langword="null"/> 
    /// collections and returning distinct elements.
    /// </summary>
    public static IEnumerable<T> Union<T>(params IEnumerable<T>[] enumerables)
    {
        return UnionAll(enumerables).Distinct();
    }

    /// <summary>
    /// Safely enumerates the elements in <paramref name="enumerable"/> without throwing an 
    /// exception if <paramref name="enumerable"/> is <see langword="null"/>.
    /// </summary>
    public static IEnumerable<T> Enumerate<T>(IEnumerable<T> enumerable)
    {
        if (enumerable == null) return Enumerable.Empty<T>();

        return enumerable;
    }
}
