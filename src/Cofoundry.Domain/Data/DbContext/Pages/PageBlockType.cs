using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Page block types represent a type of content that can be inserted into a content 
    /// region of a page which could be simple content like 'RawHtml', 'Image' or 
    /// 'PlainText'. Custom and more complex block types can be defined by a 
    /// developer. Block types are typically created when the application
    /// starts up in the auto-update process.
    /// </summary>
    public partial class PageBlockType
    {
        public PageBlockType()
        {
            PageBlockTemplates = new List<PageBlockTypeTemplate>();
            PageVersionBlocks = new List<PageVersionBlock>();
        }

        /// <summary>
        /// Database Id of this instance
        /// </summary>
        public int PageBlockTypeId { get; set; }

        /// <summary>
        /// A human readable name that is displayed when selecting block types.
        /// The name should ideally be unique but this is not enforced as long as
        /// the filename is unique.
        /// </summary>
        /// <remarks>
        /// Regarding uniqueness, an edge case might be if you wanted to have 
        /// multiple blocks, each used in different templates but with the same 
        /// name e.g. two blocks with file names 'HomepageContent' and 'AboutContent'
        /// that were both named 'Content' because int he context of the templates there 
        /// would be no conflict.
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        /// An optional paragraph of description text that is displayed in the
        /// user interface to help someone decide which block type to choose.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The name of the view file (without extension) that gets rendered 
        /// with the block data. This file name needs to be unique.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// This is basically a soft delete, with archived templates
        /// not allowed to be assigned but allowed to be kept in place 
        /// to support references to previous page versions.
        /// </summary>
        public bool IsArchived { get; set; }

        #region Auditing

        /// <summary>
        /// Date the block type was created.
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Date the block type was last updated.
        /// </summary>
        public DateTime UpdateDate { get; set; }

        #endregion

        /// <summary>
        /// A page block can optionally have one or more templates in addition to the 
        /// default template. This allows for a user to display content in multiple 
        /// ways.
        /// </summary>
        public virtual ICollection<PageBlockTypeTemplate> PageBlockTemplates { get; set; }
        
        /// <summary>
        /// The page blocks this type is used in.
        /// </summary>
        public virtual ICollection<PageVersionBlock> PageVersionBlocks { get; set; }

        /// <summary>
        /// The custom entity blocks this type is used in.
        /// </summary>
        public virtual ICollection<CustomEntityVersionPageBlock> CustomEntityVersionPageBlocks { get; set; }
    }
}
