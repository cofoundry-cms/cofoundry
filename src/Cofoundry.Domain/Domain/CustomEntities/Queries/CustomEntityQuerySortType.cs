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
    public enum CustomEntityQuerySortType
    {
        /// <summary>
        /// Sort by relevance if specified, but falls back on Natural ordering.
        /// </summary>
        Default,

        /// <summary>
        /// Ordering by a CustomEntityOrdering if one is specified, then by title.
        /// </summary>
        Natural,

        /// <summary>
        /// Orders alphabetically by the text in the Title field.
        /// </summary>
        Title,

        /// <summary>
        /// Order by locale identifier then by the default ordering.
        /// </summary>
        Locale,

        /// <summary>
        /// Orders by the create date (newest first).
        /// </summary>
        CreateDate,

        /// <summary>
        /// Orders by the publish date (latest first), for entities not yet published then 
        /// the create date is used.
        /// </summary>
        PublishDate,

        // Full -text query Not yet implemented
        /// <summary>
        /// Sort by relevance to the 
        /// </summary>
        //Relevance
    }
}
