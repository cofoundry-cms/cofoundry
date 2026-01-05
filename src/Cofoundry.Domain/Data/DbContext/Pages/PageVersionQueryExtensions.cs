namespace Cofoundry.Domain.Data;

/// <summary>
/// Extension methods for <see cref="IQueryable{PageVersion}"/>.
/// </summary>
public static class PageVersionQueryExtensions
{
    extension(IQueryable<PageVersion> pageVersions)
    {
        /// <summary>
        /// Filters the result to include only the version with the specified PageVersionId
        /// </summary>
        public IQueryable<PageVersion> FilterByPageVersionId(int pageVersionId)
        {
            var filtered = pageVersions.Where(v => v.PageVersionId == pageVersionId);

            return filtered;
        }

        /// <summary>
        /// Filters the result to include only the version with the specified PageVersionId
        /// </summary>
        public IQueryable<PageVersion> FilterByPageId(int pageId)
        {
            var filtered = pageVersions.Where(v => v.PageId == pageId);

            return filtered;
        }

        /// <summary>
        /// Filters the collection to only include versions that are
        /// not deleted.
        /// </summary>
        public IQueryable<PageVersion> FilterActive()
        {
            var filtered = pageVersions.Where(v => !v.PageTemplate.IsArchived);

            return filtered;
        }
    }
}
