using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    public static class OrderBySortDirectionExtensions
    {
        /// <summary>
        /// Does the same as a regular OrderBy clause, but inverts the direction if the SortDirection
        /// is Descending. Used to simplify sorting when applying a directionable higher level query 
        /// (.e.g by relevance/date/title) to a lower level (e.g. EF) query that may have more complex
        /// underlying primary/secondary sorting that needs inverting.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <param name="sortDirection">
        /// The direction of the higher level sort, which is simply used to invert the ordering if it is 
        /// SortDirection.Descending
        /// </param>
        /// <returns>An System.Linq.IOrderedQueryable`1 whose elements are sorted according to a key.</returns>
        public static IOrderedQueryable<TSource> OrderByWithSortDirection<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, SortDirection sortDirection)
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
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <param name="sortDirection">
        /// The direction of the higher level sort, which is simply used to invert the ordering if it is 
        /// SortDirection.Descending
        /// </param>
        /// <returns>An System.Linq.IOrderedQueryable`1 whose elements are sorted according to a key.</returns>
        public static IOrderedQueryable<TSource> OrderByDescendingWithSortDirection<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, SortDirection sortDirection)
        {
            if (sortDirection == SortDirection.Default)
            {
                return source.OrderByDescending(keySelector);
            }

            return source.OrderBy(keySelector);
        }

        /// <summary>
        /// Does the same as a regular ThenBy clause, but inverts the direction if the SortDirection
        /// is Descending. Used to simplify sorting when applying a directionable higher level query 
        /// (.e.g by relevance/date/title) to a lower level (e.g. EF) query that may have more complex
        /// underlying primary/secondary sorting that needs inverting.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <param name="source">An System.Linq.IOrderedQueryable`1 that contains elements to sort.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <param name="sortDirection">
        /// The direction of the higher level sort, which is simply used to invert the ordering if it is 
        /// SortDirection.Descending
        /// </param>
        /// <returns>An System.Linq.IOrderedQueryable`1 whose elements are sorted according to a key.</returns>
        public static IOrderedQueryable<TSource> ThenByWithSortDirection<TSource, TKey>(this IOrderedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, SortDirection sortDirection)
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
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <param name="source">An System.Linq.IOrderedQueryable`1 that contains elements to sort.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <param name="sortDirection">
        /// The direction of the higher level sort, which is simply used to invert the ordering if it is 
        /// SortDirection.Descending
        /// </param>
        /// <returns>An System.Linq.IOrderedQueryable`1 whose elements are sorted according to a key.</returns>
        public static IOrderedQueryable<TSource> ThenByDescendingWithSortDirection<TSource, TKey>(this IOrderedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, SortDirection sortDirection)
        {
            if (sortDirection == SortDirection.Default)
            {
                return source.ThenByDescending(keySelector);
            }

            return source.ThenBy(keySelector);
        }
    }
}
