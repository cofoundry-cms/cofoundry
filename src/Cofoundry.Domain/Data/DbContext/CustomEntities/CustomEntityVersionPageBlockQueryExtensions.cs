namespace Cofoundry.Domain.Data;

/// <summary>
/// Extension methods for <see cref="IQueryable{CustomEntityVersionPageBlock}"/>.
/// </summary>
public static class CustomEntityVersionPageBlockQueryExtensions
{
    extension(IQueryable<CustomEntityVersionPageBlock> pages)
    {
        /// <summary>
        /// Filters the collection to only include blocks that
        /// have not been archived.
        /// </summary>
        public IQueryable<CustomEntityVersionPageBlock> FilterActive()
        {
            var filtered = pages.Where(p => !p.PageBlockType.IsArchived);

            return filtered;
        }
    }
}
