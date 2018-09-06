using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Page block types represent a type of content that can be inserted into a content 
    /// region of a page which could be simple content like 'RawHtml', 'Image' or 
    /// 'PlainText'. Custom and more complex block types can be defined by a 
    /// developer. Block types are typically created when the application
    /// starts up in the auto-update process.
    /// </summary>
    public class PageBlockTypeSummary
    {
        /// <summary>
        /// Database id of the block type record.
        /// </summary>
        public int PageBlockTypeId { get; set; }

        /// <summary>
        /// A human readable name that is displayed when selecting block types.
        /// The name should ideally be unique but this is not enforced as long as
        /// the filename is unique.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// An optional description used to help users pick a block
        /// type from a list of options.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The name of the view file (without extension) that gets rendered 
        /// with the block data. This file name needs to be unique.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// A block can optionally have display templates associated with it, 
        /// which will give the user a choice about how the data is rendered out
        /// e.g. 'Wide', 'Headline', 'Large', 'Reversed'. If no template is set then 
        /// the default view is used for rendering.
        /// </summary>
        public ICollection<PageBlockTypeTemplateSummary> Templates { get; set; }
    }
}
