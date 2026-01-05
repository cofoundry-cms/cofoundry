namespace Cofoundry.Domain.Data;

/// <summary>
/// Extension methods for querying entities that implement <see cref="IEntityVersion"/>.
/// </summary>
public static class EntityVersionExtensions
{
    extension<T>(IEnumerable<T> source) where T : IEntityVersion
    {
        /// <summary>
        /// Orders the versions by date ensuring that the draft version is always first.
        /// </summary>
        public IOrderedEnumerable<T> OrderByLatest()
        {
            return source
                .OrderByDescending(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft)
                .ThenByDescending(v => v.CreateDate);
        }
    }

    extension<T>(IQueryable<T> source) where T : IEntityVersion
    {
        /// <summary>
        /// Orders the versions by date ensuring that the draft version is always first.
        /// </summary>
        public IOrderedQueryable<T> OrderByLatest()
        {
            return source
                .OrderByDescending(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft)
                .ThenByDescending(v => v.CreateDate);
        }
    }
}
