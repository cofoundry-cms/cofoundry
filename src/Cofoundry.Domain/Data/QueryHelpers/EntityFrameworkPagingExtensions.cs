namespace Cofoundry.Domain.Data;

/// <summary>
/// Extension methods for paging data via <see cref="IQueryable{T}"/>.
/// </summary>
public static class EntityFrameworkPagingExtensions
{
    extension<T>(IQueryable<T> source)
    {
        /// <summary>
        /// Converts a query to an instance of PagedQueryResult, executing the query twice,
        /// once to get the total count and again to get the results.
        /// </summary>
        public async Task<PagedQueryResult<T>> ToPagedResultAsync(IPageableQuery query)
        {
            ArgumentNullException.ThrowIfNull(source);

            var result = new PagedQueryResult<T>();
            result.TotalItems = await source.CountAsync();
            result.Items = await source.Page(query).ToArrayAsync();
            PagingQueryExtensions.MapPagingData<T>(query, result);

            return result;
        }
    }
}
