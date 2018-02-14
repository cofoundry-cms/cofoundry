using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Page data required to render a page, including template data for all the content-editable
    /// regions. This object is specific to a particular version which may not always be the 
    /// latest (depending on the query).
    /// </summary>
    public class PageRenderDetails
    {
        public PageRenderDetails()
        {
            OpenGraph = new OpenGraphData();
        }

        public int PageId { get; set; }
        public int PageVersionId { get; set; }

        public string Title { get; set; }
        public string MetaDescription { get; set; }

        /// <summary>
        /// WorkFlowStatus of the version that this instance represents. The version
        /// may not always be the latest version and is dependent on the query that
        /// was used to load this instance, typically using a PublishStatusQuery value.
        /// </summary>
        public WorkFlowStatus WorkFlowStatus { get; set; }

        public OpenGraphData OpenGraph { get; set; }
        public PageRoute PageRoute { get; set; }

        public PageTemplateMicroSummary Template { get; set; }
        public ICollection<PageRegionRenderDetails> Regions { get; set; }
    }
}
