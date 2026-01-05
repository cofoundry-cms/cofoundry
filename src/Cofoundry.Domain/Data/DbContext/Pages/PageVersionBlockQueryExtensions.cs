namespace Cofoundry.Domain.Data;

/// <summary>
/// Extension methods for <see cref="IQueryable{PageVersionBlock}"/>.
/// </summary>
public static class PageVersionBlockQueryExtensions
{
    extension(IQueryable<PageVersionBlock> pages)
    {
        /// <summary>
        /// Filters the collection to only include blocks that
        /// have not been archived.
        /// </summary>
        public IQueryable<PageVersionBlock> FilterActive()
        {
            var filtered = pages.Where(p => !p.PageBlockType.IsArchived);

            return filtered;
        }
    }
}
