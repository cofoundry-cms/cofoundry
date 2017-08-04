using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Contains all the information required to render a page block out to a page.
    /// </summary>
    public class PageVersionBlockRenderDetails : IEntityVersionPageBlockRenderDetails
    {
        public int PageVersionBlockId { get; set; }

        /// <summary>
        /// A block can optionally have display templates associated with it, 
        /// which will give the user a choice about how the data is rendered out.
        /// If no template is set then the default view is used for rendering.
        /// </summary>
        public PageBlockTypeTemplateSummary Template { get; set; }

        public PageBlockTypeSummary BlockType { get; set; }

        public IPageBlockTypeDisplayModel DisplayModel { get; set; }

        public int EntityVersionPageBlockId
        {
            get { return PageVersionBlockId; }
            set { PageVersionBlockId = value; }
        }
    }
}
