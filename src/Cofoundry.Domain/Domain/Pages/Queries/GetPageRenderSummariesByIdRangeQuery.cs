using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Query to get a range of pages by a set of ids, projected as a PageRenderSummary, which is
    /// a lighter weight projection designed for rendering to a site when the 
    /// templates, region and block data is not required. The results are 
    /// version-sensitive and defaults to returning published versions only, but
    /// this behavior can be controlled by the publishStatus query property.
    /// </summary>
    public class GetPageRenderSummariesByIdRangeQuery : IQuery<IDictionary<int, PageRenderSummary>>
    {
        /// <summary>
        /// Query to get a range of pages by a set of ids, projected as a PageRenderSummary, which is
        /// a lighter weight projection designed for rendering to a site when the 
        /// templates, region and block data is not required. The results are 
        /// version-sensitive and defaults to returning published versions only, but
        /// this behavior can be controlled by the publishStatus query property.
        /// </summary>
        public GetPageRenderSummariesByIdRangeQuery() { }

        /// <summary>
        /// Query to get a range of pages by a set of ids, projected as a PageRenderSummary, which is
        /// a lighter weight projection designed for rendering to a site when the 
        /// templates, region and block data is not required. The results are 
        /// version-sensitive and defaults to returning published versions only, but
        /// this behavior can be controlled by the publishStatus query property.
        /// </summary>
        /// <param name="pageIds">Database ids of the pages to get.</param>
        /// <param name="publishStatus">Used to determine which version of the page to include data for.</param>
        public GetPageRenderSummariesByIdRangeQuery(IEnumerable<int> pageIds, PublishStatusQuery? publishStatus = null)
            : this(pageIds?.ToList(), publishStatus)
        {
        }

        /// <summary>
        /// Query to get a range of pages by a set of ids, projected as a PageRenderSummary, which is
        /// a lighter weight projection designed for rendering to a site when the 
        /// templates, region and block data is not required. The results are 
        /// version-sensitive and defaults to returning published versions only, but
        /// this behavior can be controlled by the publishStatus query property.
        /// </summary>
        /// <param name="pageIds">Database ids of the pages to get.</param>
        /// <param name="publishStatus">Used to determine which version of the page to include data for.</param>
        public GetPageRenderSummariesByIdRangeQuery(IReadOnlyCollection<int> pageIds, PublishStatusQuery? publishStatus = null)
        {
            PageIds = pageIds;
            if (publishStatus.HasValue)
            {
                PublishStatus = publishStatus.Value;
            }
        }

        /// <summary>
        /// Database ids of the pages to get.
        /// </summary>
        public IReadOnlyCollection<int> PageIds { get; set; }

        /// <summary>
        /// Used to determine which version of the page to include data for. This 
        /// defaults to Published, meaning that only published pages will be returned.
        /// </summary>
        public PublishStatusQuery PublishStatus { get; set; }
    }
}
