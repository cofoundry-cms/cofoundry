using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// A block can optionally have display templates associated with it, 
    /// which will give the user a choice about how the data is rendered out
    /// e.g. 'Wide', 'Headline', 'Large', 'Reversed'. If no template is set then 
    /// the default view is used for rendering.
    /// </summary>
    public partial class PageBlockTypeTemplate
    {
        /// <summary>
        /// Database id of the block type template record.
        /// </summary>
        public int PageBlockTypeTemplateId { get; set; }

        /// <summary>
        /// Database id of the block type record this template belongs to.
        /// </summary>
        public int PageBlockTypeId { get; set; }

        /// <summary>
        /// The display name for the template in the administration UI
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// An optional description used to help users pick a block
        /// type template from a list of options.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The name of the template view file without an extensions 
        /// e.g. 'H1', 'ReversedContent'. Must be unique to the block
        /// type.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The block type this template belongs to. One block type can
        /// 0 or more templates.
        /// </summary>
        public virtual PageBlockType PageBlockType { get; set; }
        
        /// <summary>
        /// Date the record was created.
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
