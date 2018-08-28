using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents the different ways we can apply sorting to custom entity search queries
    /// </summary>
    public enum PageQuerySortType
    {
        /// <summary>
        /// Sort by relevance if specified, but falls back to title ordering
        /// </summary>
        Default,

        /// <summary>
        /// Orders alphabetically by the text in the Title field
        /// </summary>
        Title,

        /// <summary>
        /// Order by locale identifier then by the default ordering.
        /// </summary>
        Locale,

        /// <summary>
        /// Orders by the create date (newest first)
        /// </summary>
        CreateDate,

        /// <summary>
        /// Orders by the publish date (latest first), then by create date (for pages not published)
        /// </summary>
        PublishDate,

        // Full -text query Not yet implemented
        //Relevance
    }
}
