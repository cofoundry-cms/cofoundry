namespace Cofoundry.Domain.Data;

/// <summary>
/// Extension methods for querying entities that implement <see cref="ICreateable"/>.
/// </summary>
public static class ICreateableExtensions
{
    extension<TEntity>(IQueryable<TEntity> source) where TEntity : class, ICreateable
    {
        /// <summary>
        /// Filters the create date value by the specified parameters. Date 
        /// parameters can be null which indicates an unbounded query.
        /// </summary>
        /// <param name="startDate">Inclusive start date.</param>
        /// <param name="endDate">Exclusive end date to filter by.</param>
        public IQueryable<TEntity> FilterByCreateDate(DateTime? startDate, DateTime? endDate)
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
        public IQueryable<TEntity> FilterByCreateDate(DateTimeOffset? startDate, DateTimeOffset? endDate)
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
