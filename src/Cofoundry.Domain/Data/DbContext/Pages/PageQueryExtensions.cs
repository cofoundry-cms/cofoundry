namespace Cofoundry.Domain.Data;

/// <summary>
/// Extension methods for <see cref="IQueryable{Page}"/>.
/// </summary>
public static class PageQueryExtensions
{
    extension(IQueryable<Page> pages)
    {
        /// <summary>
        /// Fitlers the collection to only include pages with the 
        /// specified <paramref name="pageId"/>.
        /// </summary>
        /// <param name="pageId">PageId to filter by.</param>
        public IQueryable<Page> FilterById(int pageId)
        {
            var result = pages
                .Where(i => i.PageId == pageId);

            return result;
        }

        /// <summary>
        /// Fitlers the collection to only include pages parented to the 
        /// specified directory.
        /// </summary>
        /// <param name="pageDirectoryId">PageDirectoryId to filter by.</param>
        public IQueryable<Page> FilterByPageDirectoryId(int pageDirectoryId)
        {
            var result = pages
                .Where(i => i.PageDirectoryId == pageDirectoryId);

            return result;
        }

        /// <summary>
        /// Filters the collection to only include versions that are
        /// not deleted and not in deleted directories.
        /// </summary>
        public IQueryable<Page> FilterActive()
        {
            // Not currently filtered, but may need to add locale filtering in here
            // in a later version so will leave this here for now.
            return pages;
        }
    }
}
