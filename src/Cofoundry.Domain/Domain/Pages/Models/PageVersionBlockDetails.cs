using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Contains all of the key data for an individual block in
    /// a specific version of a page, but does not include the 
    /// mapped display model. Use the PageVersionBlockRenderDetails
    /// projection to include the mapped display model.
    /// </summary>
    public class PageVersionBlockDetails
    {
        /// <summary>
        /// Database id of the page version block record.
        /// </summary>
        public int PageVersionBlockId { get; set; }

        /// <summary>
        /// A page block can optionally have display templates associated with it, 
        /// which will give the user a choice about how the data is rendered out.
        /// If no template is set then the default view is used for rendering.
        /// </summary>
        public PageBlockTypeTemplateSummary Template { get; set; }

        /// <summary>
        /// The block type that is used to describe and render 
        /// this instance.
        /// </summary>
        public PageBlockTypeSummary BlockType { get; set; }

        /// <summary>
        /// The raw data model, deserialized from the database but
        /// not mapped to the block type display model.
        /// </summary>
        public IPageBlockTypeDataModel DataModel { get; set; }
    }
}
