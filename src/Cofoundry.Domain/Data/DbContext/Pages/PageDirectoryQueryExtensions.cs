using System.Linq;

namespace Cofoundry.Domain.Data
{
    public static class PageDirectoryQueryExtensions
    {
        /// <summary>
        /// Fitlers the collection to only include pages with the 
        /// specified <paramref name="pageDirectoryId"/>.
        /// </summary>
        /// <param name="pageDirectoryId">PageDirectoryId to filter by.</param>
        public static IQueryable<PageDirectory> FilterByPageDirectoryId(this IQueryable<PageDirectory> pageDirectories, int pageDirectoryId)
        {
            var result = pageDirectories
                .Where(i => i.PageDirectoryId == pageDirectoryId);

            return result;
        }
    }
}
