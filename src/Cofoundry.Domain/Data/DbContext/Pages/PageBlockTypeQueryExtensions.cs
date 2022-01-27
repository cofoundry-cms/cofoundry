using System.Linq;

namespace Cofoundry.Domain.Data
{
    public static class PageBlockTypeQueryExtensions
    {
        /// <summary>
        /// Filters the collection to only include block types that
        /// have not been archived.
        /// </summary>
        public static IQueryable<PageBlockType> FilterActive(this IQueryable<PageBlockType> pages)
        {
            var filtered = pages.Where(p => !p.IsArchived);

            return filtered;
        }
    }
}