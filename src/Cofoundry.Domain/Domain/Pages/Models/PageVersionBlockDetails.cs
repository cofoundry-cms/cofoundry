using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageVersionBlockDetails
    {
        public int PageVersionBlockId { get; set; }

        /// <summary>
        /// A page block can optionally have display templates associated with it, 
        /// which will give the user a choice about how the data is rendered out.
        /// If no template is set then the default view is used for rendering.
        /// </summary>
        public PageBlockTypeTemplateSummary Template { get; set; }

        public PageBlockTypeSummary BlockType { get; set; }

        public IPageBlockTypeDataModel DataModel { get; set; }
    }
}
