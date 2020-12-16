using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Page block data for a specific custom entity version on a custom entity
    /// details page. This model contains a fully mapped display model, mapped
    /// from the unstructured data model data from the the database.
    /// </summary>
    public class CustomEntityVersionPageBlockRenderDetails : IEntityVersionPageBlockRenderDetails
    {
        /// <summary>
        /// Database id and auto-incrementing primary key of the 
        /// custom entity version block record.
        /// </summary>
        public int CustomEntityVersionPageBlockId { get; set; }
        
        /// <summary>
        /// A block can optionally have display templates associated with it, 
        /// which will give the user a choice about how the data is rendered out
        /// e.g. 'Wide', 'Headline', 'Large', 'Reversed'. If no template is set then 
        /// the default view is used for rendering.
        /// </summary>
        public PageBlockTypeTemplateSummary Template { get; set; }

        /// <summary>
        /// The block type which defines the data model and display
        /// templates available to render the block e.g. 'Image', 
        /// 'Vimeo Video', 'Heading', 'Split Content'.
        /// </summary>
        public PageBlockTypeSummary BlockType { get; set; }

        /// <summary>
        /// Custom data associated with this block, mapped from
        /// the serialized database data into a display model.
        /// </summary>
        public IPageBlockTypeDisplayModel DisplayModel { get; set; }

        /// <summary>
        /// Abstraction of the database identifier for this instance. Used
        /// to abstract away the two implementations of page block data - for
        /// pages and custom entities.
        /// </summary>
        public int EntityVersionPageBlockId
        {
            get { return CustomEntityVersionPageBlockId; }
            set { CustomEntityVersionPageBlockId = value; }
        }
    }
}
