using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Page data required to render a page, including template data for all the content-editable
    /// sections. This object is specific to a particular version which may not always be the 
    /// latest (depending on the query).
    /// </summary>
    public class PageRenderDetails
    {
        public PageRenderDetails()
        {
            MetaData = new PageMetaData();
            OpenGraph = new OpenGraphData();
        }

        public int PageId { get; set; }
        public int PageVersionId { get; set; }

        public string Title { get; set; }
        public WorkFlowStatus WorkFlowStatus { get; set; }

        public PageMetaData MetaData { get; set; }
        public OpenGraphData OpenGraph { get; set; }

        public PageTemplateMicroSummary Template { get; set; }
        public IEnumerable<PageSectionRenderDetails> Sections { get; set; }
    }
}
