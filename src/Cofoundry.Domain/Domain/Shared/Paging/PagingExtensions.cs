using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public static class PagingExtensions
    {
        #region queries

        /// <summary>
        /// Pages a query based on the parameters of the query.
        /// </summary>
        public static IQueryable<T> Page<T>(this IQueryable<T> source, IPageableQuery query)
        {
            if (query == null || query.PageSize <= 0) return source;

            var pageNumber = query.PageNumber < 1 ? 0 : query.PageNumber -1;
            var itemsToSkip = pageNumber * query.PageSize;

            return source
                .Skip(itemsToSkip)
                .Take(query.PageSize);
        }

        /// <summary>
        /// Converts a query to an instance of PagedQueryResult, executing the query twice,
        /// once to get the total count and again to get the results.
        /// </summary>
        public static PagedQueryResult<T> ToPagedResult<T>(this IQueryable<T> source, IPageableQuery query)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var result = new PagedQueryResult<T>();
            result.TotalItems = source.Count();
            result.Items = source.Page(query).ToArray();
            MapPagingData<T>(query, result);

            return result;
        }

        #region helpers

        /// <remarks>
        /// Public so it can be referenced by ToPagedResultAsync in Cofoundry.Domain.Data
        /// </remarks>
        public static void MapPagingData<T>(IPageableQuery query, PagedQueryResult<T> result)
        {
            if (query != null && query.PageSize > 0)
            {
                result.PageSize = query.PageSize;
                result.PageCount = (int)Math.Ceiling(result.TotalItems / (double)result.PageSize);
                result.PageNumber = query.PageNumber < 1 ? 1 : query.PageNumber;
            }
            else
            {
                result.PageSize = result.TotalItems;
                result.PageCount = 1;
                result.PageNumber = 1;
            }
        }

        #endregion

        #endregion
    }
}
