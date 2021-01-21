using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain.Data
{
    public static class ICreateableExtensions
    {
        /// <summary>
        /// Filters the create date value by the specified parameters. Date 
        /// parameters can be null which indicates an unbounded query.
        /// </summary>
        /// <param name="startDate">Inclusive start date.</param>
        /// <param name="endDate">Exclusive end date to filter by.</param>
        public static IQueryable<TEntity> FilterByCreateDate<TEntity>(this IQueryable<TEntity> source, DateTime? startDate, DateTime? endDate)
            where TEntity : class, ICreateable
        {
            if (startDate.HasValue)
            {
                source = source.Where(c => c.CreateDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                source = source.Where(c => c.CreateDate < endDate.Value);
            }

            return source;
        }

        /// <summary>
        /// Filters the create date value by the specified parameters. Date 
        /// parameters can be null which indicates an unbounded query.
        /// </summary>
        /// <param name="startDate">Inclusive start date.</param>
        /// <param name="endDate">Exclusive end date to filter by.</param>
        public static IQueryable<TEntity> FilterByCreateDate<TEntity>(this IQueryable<TEntity> source, DateTimeOffset? startDate, DateTimeOffset? endDate)
            where TEntity : class, ICreateable
        {
            if (startDate.HasValue)
            {
                source = source.Where(c => c.CreateDate >= startDate.Value.UtcDateTime);
            }

            if (endDate.HasValue)
            {
                source = source.Where(c => c.CreateDate < endDate.Value.UtcDateTime);
            }

            return source;
        }
    }
}
