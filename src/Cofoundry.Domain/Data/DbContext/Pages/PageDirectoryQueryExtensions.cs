namespace Cofoundry.Domain.Data;

/// <summary>
/// Extension methods for <see cref="IQueryable{PageDirectory}"/>.
/// </summary>
public static class PageDirectoryQueryExtensions
{
    extension(IQueryable<PageDirectory> pageDirectories)
    {
        /// <summary>
        /// Fitlers the collection to only include pages with the 
        /// specified <paramref name="pageDirectoryId"/> primary key.
        /// </summary>
        /// <param name="pageDirectoryId">PageDirectoryId to filter by.</param>
        public IQueryable<PageDirectory> FilterById(int pageDirectoryId)
        {
            var result = pageDirectories.Where(i => i.PageDirectoryId == pageDirectoryId);

            return result;
        }
    }

    extension(IEnumerable<PageDirectory> pageDirectories)
    {
        /// <summary>
        /// Fitlers the collection to only include pages with the 
        /// specified <paramref name="pageDirectoryId"/> primary key.
        /// </summary>
        /// <param name="pageDirectoryId">PageDirectoryId to filter by.</param>
        public IEnumerable<PageDirectory> FilterById(int pageDirectoryId)
        {
            var result = pageDirectories.Where(i => i.PageDirectoryId == pageDirectoryId);

            return result;
        }
    }
}
