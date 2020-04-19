using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Page block data for a specific page version. Each page block appears 
    /// in a template region. The data and rendering of each block is controlled 
    /// by the page block type assigned to it.
    /// </summary>
    public partial class PageVersionBlock : ICreateAuditable, IEntityVersionPageBlock
    {
        /// <summary>
        /// Auto-incrementing primary key of the record in the database.
        /// </summary>
        public int PageVersionBlockId { get; set; }

        /// <summary>
        /// Id of the page version that this instance is
        /// parented to.
        /// </summary>
        public int PageVersionId { get; set; }

        /// <summary>
        /// Id of the template region this block belongs to.
        /// </summary>
        public int PageTemplateRegionId { get; set; }

        /// <summary>
        /// Id of the block type which defines the data model and display
        /// templates available to render the block e.g. 'Image', 
        /// 'Vimeo Video', 'Heading', 'Split Content'.
        /// </summary>
        public int PageBlockTypeId { get; set; }

        /// <summary>
        /// The block data model serialized into string data by
        /// IDbUnstructuredDataSerializer, which uses JSON serlialization
        /// by default.
        /// </summary>
        public string SerializedData { get; set; }

        /// <summary>
        /// In regions that support multiple block types this indicates
        /// the ordering of modules in the region.
        /// </summary>
        public int Ordering { get; set; }

        /// <summary>
        /// A block can optionally have display templates associated with it, 
        /// which will give the user a choice about how the data is rendered out
        /// e.g. 'Wide', 'Headline', 'Large', 'Reversed'. If no template is set then 
        /// the default view is used for rendering.
        /// </summary>
        public int? PageBlockTypeTemplateId { get; set; }

        /// <summary>
        /// The template region this block belongs to.
        /// </summary>
        public virtual PageTemplateRegion PageTemplateRegion { get; set; }

        /// <summary>
        /// The block type which defines the data model and display
        /// templates available to render the block e.g. 'Image', 
        /// 'Vimeo Video', 'Heading', 'Split Content'.
        /// </summary>
        public virtual PageBlockType PageBlockType { get; set; }

        /// <summary>
        /// The page version that this instance is
        /// parented to.
        /// </summary>
        public virtual PageVersion PageVersion { get; set; }

        /// <summary>
        /// A block can optionally have display templates associated with it, 
        /// which will give the user a choice about how the data is rendered out
        /// e.g. 'Wide', 'Headline', 'Large', 'Reversed'. If no template is set then 
        /// the default view is used for rendering.
        /// </summary>
        public virtual PageBlockTypeTemplate PageBlockTypeTemplate { get; set; }

        #region ICreateAuditable

        public System.DateTime CreateDate { get; set; }
        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }

        #endregion

        /// <summary>
        /// Date that the block was last updated.
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Returns the id and database primary key of this instance.
        /// </summary>
        public int GetVersionBlockId()
        {
            return PageVersionBlockId;
        }
    }
}
