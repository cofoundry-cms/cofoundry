using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Abstraction of the common propeties of CustomEntityPageBlock 
    /// and PageVersionBlock.
    /// </summary>
    public interface IEntityVersionPageBlock : IEntityOrderable
    {
        /// <summary>
        /// Id of the template region this block belongs to.
        /// </summary>
        int PageTemplateRegionId { get; set; }

        /// <summary>
        /// Id of the block type which defines the data model and display
        /// templates available to render the block e.g. 'Image', 
        /// 'Vimeo Video', 'Heading', 'Split Content'.
        /// </summary>
        int PageBlockTypeId { get; set; }

        /// <summary>
        /// A block can optionally have display templates associated with it, 
        /// which will give the user a choice about how the data is rendered out
        /// e.g. 'Wide', 'Headline', 'Large', 'Reversed'. If no template is set then 
        /// the default view is used for rendering.
        /// </summary>
        int? PageBlockTypeTemplateId { get; set; }

        /// <summary>
        /// The block data model serialized into string data by
        /// IDbUnstructuredDataSerializer, which uses JSON serlialization
        /// by default.
        /// </summary>
        string SerializedData { get; set; }

        /// <summary>
        /// The template region this block belongs to.
        /// </summary>
        PageTemplateRegion PageTemplateRegion { get; set; }

        /// <summary>
        /// The block type which defines the data model and display
        /// templates available to render the block e.g. 'Image', 
        /// 'Vimeo Video', 'Heading', 'Split Content'.
        /// </summary>
        PageBlockType PageBlockType { get; set; }

        /// <summary>
        /// A block can optionally have display templates associated with it, 
        /// which will give the user a choice about how the data is rendered out
        /// e.g. 'Wide', 'Headline', 'Large', 'Reversed'. If no template is set then 
        /// the default view is used for rendering.
        /// </summary>
        PageBlockTypeTemplate PageBlockTypeTemplate { get; set; }

        /// <summary>
        /// Returns the id and database primary key of this instance.
        /// </summary>
        int GetVersionBlockId();
    }
}
