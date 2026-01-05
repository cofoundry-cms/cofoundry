namespace Cofoundry.Domain.Data;

/// <summary>
/// Extension methods for <see cref="IQueryable{PageDirectoryClosure}"/>.
/// </summary>
public static class PageDirectoryClosureQueryExtensions
{
    extension(IQueryable<PageDirectoryClosure> closures)
    {
        /// <summary>
        /// Fitlers the collection based on the on the DescendantPageDirectoryId 
        /// field i.e. this will only return the self-referencing record and
        /// ancestors of the specified directory.
        /// </summary>
        /// <param name="pageDirectoryId">PageDirectoryId to filter by on the DescendantPageDirectoryId field.</param>
        public IQueryable<PageDirectoryClosure> FilterByDescendantId(int pageDirectoryId)
        {
            var result = closures.Where(i => i.DescendantPageDirectoryId == pageDirectoryId);

            return result;
        }

        /// <summary>
        /// Fitlers the collection based on the on the AncestorPageDirectoryId 
        /// field i.e. this will only return the self-referencing record and
        /// decendants of the specified directory.
        /// </summary>
        /// <param name="pageDirectoryId">PageDirectoryId to filter by on the AncestorPageDirectoryId field.</param>
        public IQueryable<PageDirectoryClosure> FilterByAncestorId(int pageDirectoryId)
        {
            var result = closures.Where(i => i.AncestorPageDirectoryId == pageDirectoryId);

            return result;
        }

        /// <summary>
        /// Filters to include only the self-referencing records, where the ancestor and descenent are the same.
        /// </summary>
        public IQueryable<PageDirectoryClosure> FilterSelfReferencing()
        {
            var result = closures.Where(i => i.AncestorPageDirectoryId == i.DescendantPageDirectoryId);

            return result;
        }

        /// <summary>
        /// Removes the self-referencing records, where the ancestor and descendant are the same.
        /// </summary>
        public IQueryable<PageDirectoryClosure> FilterNotSelfReferencing()
        {
            var result = closures.Where(i => i.AncestorPageDirectoryId != i.DescendantPageDirectoryId);

            return result;
        }
    }

    extension(IEnumerable<PageDirectoryClosure> closures)
    {
        /// <summary>
        /// Fitlers the collection based on the on the DescendantPageDirectoryId 
        /// field i.e. this will only return the self-referencing record and
        /// ancestors of the specified directory.
        /// </summary>
        /// <param name="pageDirectoryId">PageDirectoryId to filter by on the DescendantPageDirectoryId field.</param>
        public IEnumerable<PageDirectoryClosure> FilterByDescendantId(int pageDirectoryId)
        {
            var result = closures.Where(i => i.DescendantPageDirectoryId == pageDirectoryId);

            return result;
        }

        /// <summary>
        /// Fitlers the collection based on the on the AncestorPageDirectoryId 
        /// field i.e. this will only return the self-referencing record and
        /// decendants of the specified directory.
        /// </summary>
        /// <param name="pageDirectoryId">PageDirectoryId to filter by on the AncestorPageDirectoryId field.</param>
        public IEnumerable<PageDirectoryClosure> FilterByAncestorId(int pageDirectoryId)
        {
            var result = closures.Where(i => i.AncestorPageDirectoryId == pageDirectoryId);

            return result;
        }

        /// <summary>
        /// Filters to include only the self-referencing records, where the ancestor and descenent are the same.
        /// </summary>
        public IEnumerable<PageDirectoryClosure> FilterSelfReferencing()
        {
            var result = closures.Where(i => i.AncestorPageDirectoryId == i.DescendantPageDirectoryId);

            return result;
        }

        /// <summary>
        /// Removes the self-referencing records, where the ancestor and descendant are the same.
        /// </summary>
        public IEnumerable<PageDirectoryClosure> FilterNotSelfReferencing()
        {
            var result = closures.Where(i => i.AncestorPageDirectoryId != i.DescendantPageDirectoryId);

            return result;
        }
    }
}
