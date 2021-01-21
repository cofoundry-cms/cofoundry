using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Cofoundry.Domain.Data
{
    public static class IQueryableExtensions
    {
        /// <summary>
        /// Filters the collection by a date value using the specified parameters. Date 
        /// parameters can be null which indicates an unbounded query.
        /// </summary>
        /// <param name="selector">Date field selector to filter on.</param>
        /// <param name="startDate">Inclusive start date.</param>
        /// <param name="endDate">Exclusive end date to filter by.</param>
        public static IQueryable<TEntity> FilterByDate<TEntity>(
            this IQueryable<TEntity> source,
            Expression<Func<TEntity, DateTime>> selector,
            DateTime? startDate,
            DateTime? endDate
            )
        {
            return source.ApplyFilterByDate(selector, startDate, endDate);
        }

        /// <summary>
        /// Filters the collection by a date value using the specified parameters. Date 
        /// parameters can be null which indicates an unbounded query.
        /// </summary>
        /// <param name="selector">Date field selector to filter on.</param>
        /// <param name="startDate">Inclusive start date.</param>
        /// <param name="endDate">Exclusive end date to filter by.</param>
        public static IQueryable<TEntity> FilterByDate<TEntity>(this IQueryable<TEntity> source, Expression<Func<TEntity, DateTime>> selector, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            return source.ApplyFilterByDate(selector, startDate?.UtcDateTime, endDate?.UtcDateTime);
        }

        private static IQueryable<TEntity> ApplyFilterByDate<TEntity>(
            this IQueryable<TEntity> source,
            Expression<Func<TEntity, DateTime>> selector,
            DateTime? startDate,
            DateTime? endDate
            )
        {
            if (startDate.HasValue)
            {
                var predicate = Expression.Lambda<Func<TEntity, bool>>(
                   Expression.GreaterThanOrEqual(selector.Body, Expression.Constant(startDate.Value)),
                   selector.Parameters
                );

                source = source.Where(predicate);
            }

            if (endDate.HasValue)
            {
                var predicate = Expression.Lambda<Func<TEntity, bool>>(
                   Expression.LessThan(selector.Body, Expression.Constant(endDate.Value)),
                   selector.Parameters
                );
                source = source.Where(predicate);
            }

            return source;
        }
    }
}
