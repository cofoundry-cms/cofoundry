namespace Cofoundry.Domain.Data;

/// <summary>
/// Extension methods for <see cref="IQueryable{PageBlockType}"/>.
/// </summary>
public static class PageBlockTypeQueryExtensions
{
    extension(IQueryable<PageBlockType> pages)
    {
        /// <summary>
        /// Filters the collection to only include block types that
        /// have not been archived.
        /// </summary>
        public IQueryable<PageBlockType> FilterActive()
        {
            var filtered = pages.Where(p => !p.IsArchived);

            return filtered;
        }
    }
}
