using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public static class IPageableQueryExtensions
    {
        /// <summary>
        /// Sets the paging bounds of an IPageableQuery, setting a default page
        /// size if one isn't provided.
        /// </summary>
        /// <param name="query">Query to set the bounds of.</param>
        /// <param name="defaultSize">The default page size to apply if one is not set.</param>
        /// <param name="allowUnbounded">
        /// By default negative page sizes indicate that no paging should apply, but
        /// this setting can be used to prevent that behaviour. The default value is
        /// false, which means the default page size will be applied if the page size
        /// less than 0. If set to true the default page size will only be applied if 
        /// the page size is 0 i.e. negative page sizes will be permitted.
        /// </param>
        public static void SetBounds(
            this IPageableQuery query,
            int defaultSize,
            bool allowUnbounded = false
            )
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            if ((allowUnbounded && query.PageSize == 0) ||
                (!allowUnbounded && query.PageSize <= 0))
            {
                query.PageSize = defaultSize;
            }
        }

        /// <summary>
        /// Sets the paging bounds of an IPageableQuery, setting a default page
        /// size if one isn't provided. This overload treats negative page sizes
        /// as 'not set' rather than unbounded. It is assumed that if you're setting
        /// a max size that you don't want to allow unbounded queries.
        /// </summary>
        /// <param name="query">Query to set the bounds of.</param>
        /// <param name="defaultSize">The default page size to apply if one is not set.</param>
        /// <param name="maxSize">
        /// The maximum page size limit to apply. If the PageSize setting is exceeds
        /// this value then the PageSize is set to the maxSize value.
        /// </param>
        public static void SetBounds(
            this IPageableQuery query,
            int defaultSize,
            int maxSize
            )
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            if (query.PageSize <= 0) query.PageSize = defaultSize;
            if (query.PageSize > maxSize) query.PageSize = maxSize;
        }
    }
}
