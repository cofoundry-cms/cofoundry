using System;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents a PageVersionBlock command that requires the
    /// dynamic data model
    /// </summary>
    public interface IPageVersionBlockDataModelCommand
    {
        /// <summary>
        /// The model data to save against the block.
        /// </summary>
        IPageBlockTypeDataModel DataModel { get; set; }

        /// <summary>
        /// The id of the block type being set.
        /// </summary>
        int PageBlockTypeId { get; set; }

        /// <summary>
        /// Optional id of the block type template to render the 
        /// block data into. Some block types have multiple rendering 
        /// templates to choose from.
        /// </summary>
        int? PageBlockTypeTemplateId { get; set; }
    }
}
