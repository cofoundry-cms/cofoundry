using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Updates an existing block within a template region 
    /// of a page.
    /// </summary>
    public class UpdatePageVersionBlockCommand : ICommand, ILoggableCommand, IPageVersionBlockDataModelCommand
    {
        /// <summary>
        /// Id of the block to update.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int PageVersionBlockId { get; set; }

        /// <summary>
        /// The id of the block type to use.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int PageBlockTypeId { get; set; }

        /// <summary>
        /// Optional id of the block type template to render the 
        /// block data into. Some block types have multiple rendering 
        /// templates to choose from.
        /// </summary>
        [PositiveInteger]
        public int? PageBlockTypeTemplateId { get; set; }

        /// <summary>
        /// The block type model data to save against the block.
        /// </summary>
        [Required]
        [ValidateObject]
        public IPageBlockTypeDataModel DataModel { get; set; }
    }
}
