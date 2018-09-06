using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    public static class EntityFrameworkPagingExtensions
    {
        /// <summary>
        /// Converts a query to an instance of PagedQueryResult, executing the query twice,
        /// once to get the total count and again to get the results.
        /// </summary>
        public static async Task<PagedQueryResult<T>> ToPagedResultAsync<T>(this IQueryable<T> source, IPageableQuery query)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var result = new PagedQueryResult<T>();
            result.TotalItems = await source.CountAsync();
            result.Items = await source.Page(query).ToArrayAsync();
            PagingQueryExtensions.MapPagingData<T>(query, result);

            return result;
        }
    }
}
