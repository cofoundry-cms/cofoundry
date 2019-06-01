using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Search page data returning the PageRenderSummary projection, which is
    /// a lighter weight projection designed for rendering to a site when the 
    /// templates, region and block data is not required. The query is 
    /// version-sensitive and defaults to returning published versions only, but
    /// this behavior can be controlled by the PublishStatus query property.
    /// </summary>
    public class SearchPageRenderSummariesQuery 
        : SimplePageableQuery
        , IQuery<PagedQueryResult<PageRenderSummary>>
    {
        /// <summary>
        /// Locale id to filter the results by, if null then only entities
        /// with a null locale are shown
        /// </summary>
        public int? LocaleId { get; set; }

        /// <summary>
        /// By default the query will filter to published pages only. Use this option
        /// to change this behaviour.
        /// </summary>
        public PublishStatusQuery PublishStatus { get; set; }

        /// <summary>
        /// Optionally filter by the directory the page is parented to.
        /// </summary>
        public int? PageDirectoryId { get; set; }

        /// <summary>
        /// Option to reverse the direction of the sort.
        /// </summary>
        public SortDirection SortDirection { get; set; }

        /// <summary>
        /// The default sort is title ordering, but it can be changed
        /// with this option.
        /// </summary>
        public PageQuerySortType SortBy { get; set; }
    }
}
