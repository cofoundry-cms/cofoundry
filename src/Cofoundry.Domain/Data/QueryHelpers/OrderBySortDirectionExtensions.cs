using System.Linq.Expressions;

namespace Cofoundry.Domain.Data;

/// <summary>
/// Extension methods for sorting data via <see cref="IQueryable{TSource}"/> using the
/// Cofoundry <see cref="SortDirection"/> enum.
/// </summary>
public static class OrderBySortDirectionExtensions
{
    extension<TSource>(IQueryable<TSource> source)
    {
        /// <summary>
        /// Does the same as a regular OrderBy clause, but inverts the direction if the SortDirection
        /// is Descending. Used to simplify sorting when applying a directionable higher level query 
        /// (.e.g by relevance/date/title) to a lower level (e.g. EF) query that may have more complex
        /// underlying primary/secondary sorting that needs inverting.
        /// </summary>
        /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <param name="sortDirection">
        /// The direction of the higher level sort, which is simply used to invert the ordering if it is 
        /// SortDirection.Descending
        /// </param>
        /// <returns>An System.Linq.IOrderedQueryable`1 whose elements are sorted according to a key.</returns>
        public IOrderedQueryable<TSource> OrderByWithSortDirection<TKey>(Expression<Func<TSource, TKey>> keySelector, SortDirection sortDirection)
        {
            if (sortDirection == SortDirection.Default)
            {
                return source.OrderBy(keySelector);
            }

            return source.OrderByDescending(keySelector);
        }

        /// <summary>
        /// Does the same as a regular OrderByDescending clause, but inverts the direction if the SortDirection
        /// is Descending. Used to simplify sorting when applying a directionable higher level query 
        /// (.e.g by relevance/date/title) to a lower level (e.g. EF) query that may have more complex
        /// underlying primary/secondary sorting that needs inverting.
        /// </summary>
        /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <param name="sortDirection">
        /// The direction of the higher level sort, which is simply used to invert the ordering if it is 
        /// SortDirection.Descending
        /// </param>
        /// <returns>An System.Linq.IOrderedQueryable`1 whose elements are sorted according to a key.</returns>
        public IOrderedQueryable<TSource> OrderByDescendingWithSortDirection<TKey>(Expression<Func<TSource, TKey>> keySelector, SortDirection sortDirection)
        {
            if (sortDirection == SortDirection.Default)
            {
                return source.OrderByDescending(keySelector);
            }

            return source.OrderBy(keySelector);
        }
    }

    extension<TSource>(IOrderedQueryable<TSource> source)
    {
        /// <summary>
        /// Does the same as a regular ThenBy clause, but inverts the direction if the SortDirection
        /// is Descending. Used to simplify sorting when applying a directionable higher level query 
        /// (.e.g by relevance/date/title) to a lower level (e.g. EF) query that may have more complex
        /// underlying primary/secondary sorting that needs inverting.
        /// </summary>
        /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <param name="sortDirection">
        /// The direction of the higher level sort, which is simply used to invert the ordering if it is 
        /// SortDirection.Descending
        /// </param>
        /// <returns>An System.Linq.IOrderedQueryable`1 whose elements are sorted according to a key.</returns>
        public IOrderedQueryable<TSource> ThenByWithSortDirection<TKey>(Expression<Func<TSource, TKey>> keySelector, SortDirection sortDirection)
        {
            if (sortDirection == SortDirection.Default)
            {
                return source.ThenBy(keySelector);
            }

            return source.ThenByDescending(keySelector);
        }

        /// <summary>
        /// Does the same as a regular ThenByDescending clause, but inverts the direction if the SortDirection
        /// is Descending. Used to simplify sorting when applying a directionable higher level query 
        /// (.e.g by relevance/date/title) to a lower level (e.g. EF) query that may have more complex
        /// underlying primary/secondary sorting that needs inverting.
        /// </summary>
        /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <param name="sortDirection">
        /// The direction of the higher level sort, which is simply used to invert the ordering if it is 
        /// SortDirection.Descending
        /// </param>
        /// <returns>An System.Linq.IOrderedQueryable`1 whose elements are sorted according to a key.</returns>
        public IOrderedQueryable<TSource> ThenByDescendingWithSortDirection<TKey>(Expression<Func<TSource, TKey>> keySelector, SortDirection sortDirection)
        {
            if (sortDirection == SortDirection.Default)
            {
                return source.ThenByDescending(keySelector);
            }

            return source.ThenBy(keySelector);
        }
    }
}
