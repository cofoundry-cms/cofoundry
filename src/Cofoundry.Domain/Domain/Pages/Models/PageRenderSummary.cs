using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A lighter weight page data projection designed for rendering to a site when the 
    /// templates, region and block data is not required. This object is specific to a 
    /// particular version which may not always be the latest (depending on the query).
    /// </summary>
    public class PageRenderSummary
    {
        /// <summary>
        /// A lighter weight page data projection designed for rendering to a site when the 
        /// templates, region and block data is not required. This object is specific to a 
        /// particular version which may not always be the latest (depending on the query).
        /// </summary>
        public PageRenderSummary()
        {
            OpenGraph = new OpenGraphData();
        }

        /// <summary>
        /// The database id of the page.
        /// </summary>
        public int PageId { get; set; }

        /// <summary>
        /// The database id of the page version this instance has been mapped 
        /// to. The version which may not always be the latest (depending on the query).
        /// </summary>
        public int PageVersionId { get; set; }

        /// <summary>
        /// The descriptive human-readable title of the page that is often 
        /// used in the html page title meta tag. Does not have to be
        /// unique.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The description of the content of the page. This is intended to
        /// be used in the description html meta tag.
        /// </summary>
        public string MetaDescription { get; set; }

        /// <summary>
        /// WorkFlowStatus of the version that this instance represents. The version
        /// may not always be the latest version and is dependent on the query that
        /// was used to load this instance, typically using a PublishStatusQuery value.
        /// </summary>
        public WorkFlowStatus WorkFlowStatus { get; set; }

        public OpenGraphData OpenGraph { get; set; }

        /// <summary>
        /// The routing data for the page.
        /// </summary>
        public PageRoute PageRoute { get; set; }

        /// <summary>
        /// The date the custom entity was created.
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
