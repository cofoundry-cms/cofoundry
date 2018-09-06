using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain.Data
{
    public static class EntityVersionExtensions
    {
        /// <summary>
        /// Orders the versions by date ensuring that the draft version is always first.
        /// </summary>
        public static IOrderedEnumerable<T> OrderByLatest<T>(this IEnumerable<T> source)
            where T : IEntityVersion
        {
            return source
                .OrderByDescending(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft)
                .ThenByDescending(v => v.CreateDate);
        }

        /// <summary>
        /// Orders the versions by date ensuring that the draft version is always first.
        /// </summary>
        public static IOrderedQueryable<T> OrderByLatest<T>(this IQueryable<T> source)
            where T : IEntityVersion
        {
            return source
                .OrderByDescending(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft)
                .ThenByDescending(v => v.CreateDate);
        }
    }
}
