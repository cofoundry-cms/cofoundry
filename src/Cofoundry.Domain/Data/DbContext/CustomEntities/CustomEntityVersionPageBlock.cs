using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Page block data for a specific custom entity version on a custom entity
    /// details page.
    /// </summary>
    public class CustomEntityVersionPageBlock : IEntityOrderable, IEntityVersionPageBlock
    {
        /// <summary>
        /// Auto-incrementing primary key of the record in the database.
        /// </summary>
        public int CustomEntityVersionPageBlockId { get; set; }

        /// <summary>
        /// Id of the custom entity version that this instance is
        /// parented to.
        /// </summary>
        public int CustomEntityVersionId { get; set; }

        /// <summary>
        /// It's unlikely but there can be multiple pages using the same
        /// template for a custom entity type, so the page id is required 
        /// here so we know which page the block belongs to.
        /// </summary>
        public int PageId { get; set; }

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
        /// A block can optionally have display templates associated with it, 
        /// which will give the user a choice about how the data is rendered out
        /// e.g. 'Wide', 'Headline', 'Large', 'Reversed'. If no template is set then 
        /// the default view is used for rendering.
        /// </summary>
        public int? PageBlockTypeTemplateId { get; set; }

        /// <summary>
        /// The block data model serialized into string data by
        /// IDbUnstructuredDataSerializer, which uses JSON serlialization
        /// by default.
        /// </summary>
        public string SerializedData { get; set; }

        /// <summary>
        /// In regions that support multiple block types this indicates
        /// th ordering of modules in the region.
        /// </summary>
        public int Ordering { get; set; }

        /// <summary>
        /// The custom entity version that this instance is
        /// parented to.
        /// </summary>
        public virtual CustomEntityVersion CustomEntityVersion { get; set; }

        /// <summary>
        /// It's unlikely but there can be multiple pages using the same
        /// template for a custom entity type, so a page reference is required 
        /// here so we know which page the block belongs to.
        /// </summary>
        public virtual Page Page { get; set; }

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
        /// A block can optionally have display templates associated with it, 
        /// which will give the user a choice about how the data is rendered out
        /// e.g. 'Wide', 'Headline', 'Large', 'Reversed'. If no template is set then 
        /// the default view is used for rendering.
        /// </summary>
        public virtual PageBlockTypeTemplate PageBlockTypeTemplate { get; set; }

        /// <summary>
        /// Returns the id and database primary key of this instance.
        /// </summary>
        public int GetVersionBlockId()
        {
            return CustomEntityVersionPageBlockId;
        }
    }
}
