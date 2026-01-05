namespace Cofoundry.Domain;

/// <summary>
/// Extension methods for <see cref="IPageableQuery"/>.
/// </summary>
public static class IPageableQueryExtensions
{
    extension(IPageableQuery query)
    {
        /// <summary>
        /// Sets the paging bounds of an <see cref="IPageableQuery"/>, setting a default page
        /// size if one isn't provided.
        /// </summary>
        /// <param name="defaultSize">The default page size to apply if one is not set.</param>
        /// <param name="allowUnbounded">
        /// By default negative page sizes indicate that no paging should apply, but
        /// this setting can be used to prevent that behaviour. The default value is
        /// false, which means the default page size will be applied if the page size
        /// less than 0. If set to true the default page size will only be applied if 
        /// the page size is 0 i.e. negative page sizes will be permitted.
        /// </param>
        public void SetBounds(int defaultSize, bool allowUnbounded = false)
        {
            ArgumentNullException.ThrowIfNull(query);

            if ((allowUnbounded && query.PageSize == 0) ||
                (!allowUnbounded && query.PageSize <= 0))
            {
                query.PageSize = defaultSize;
            }
        }

        /// <summary>
        /// Sets the paging bounds of an <see cref="IPageableQuery"/>, setting a default page
        /// size if one isn't provided. This overload treats negative page sizes
        /// as 'not set' rather than unbounded. It is assumed that if you're setting
        /// a max size that you don't want to allow unbounded queries.
        /// </summary>
        /// <param name="defaultSize">The default page size to apply if one is not set.</param>
        /// <param name="maxSize">
        /// The maximum page size limit to apply. If the PageSize setting is exceeds
        /// this value then the PageSize is set to the maxSize value.
        /// </param>
        public void SetBounds(int defaultSize, int maxSize)
        {
            ArgumentNullException.ThrowIfNull(query);

            if (query.PageSize <= 0)
            {
                query.PageSize = defaultSize;
            }

            if (query.PageSize > maxSize)
            {
                query.PageSize = maxSize;
            }
        }
    }
}
