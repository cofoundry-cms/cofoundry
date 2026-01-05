namespace Cofoundry.Domain;

/// <summary>
/// Extension methods for <see cref="IPagedQueryResult"/>.
/// </summary>
public static class IPagedQueryResultExtensions
{
    extension(IPagedQueryResult source)
    {
        /// <summary>
        /// <see langword="true"/> if the result contains the first page of results. Page numbering
        /// is 1-based, but the check counts anything less than 1 as the first page.
        /// </summary>
        public bool IsFirstPage()
        {
            return source.PageNumber <= 1;
        }

        /// <summary>
        /// <see langword="true"/> if the result is the last page of the result set.
        /// </summary>
        public bool IsLastPage()
        {
            return source.PageCount <= source.PageNumber;
        }
    }
}
