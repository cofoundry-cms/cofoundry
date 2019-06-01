using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Adds a new block to a template region on a page.
    /// </summary>
    public class AddPageVersionBlockCommand : ICommand, ILoggableCommand, IValidatableObject, IPageVersionBlockDataModelCommand
    {
        /// <summary>
        /// The page version to add the block to. This must be the 
        /// draft version of a page.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int PageVersionId { get; set; }

        /// <summary>
        /// The id of the block type being added.
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
        /// The block type model data to save against the new block.
        /// </summary>
        [Required]
        [ValidateObject]
        public IPageBlockTypeDataModel DataModel { get; set; }

        /// <summary>
        /// The region of the page template to add the block to.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int PageTemplateRegionId { get; set; }

        /// <summary>
        /// Indicates where to place the block in the existing collection
        /// of blocks in the region.
        /// </summary>
        public OrderedItemInsertMode InsertMode { get; set; }

        /// <summary>
        /// When inserting before or after an block, this id indicates
        /// the reference block to use when working out placement.
        /// </summary>
        [PositiveInteger]
        public int? AdjacentVersionBlockId { get; set; }

        #region Ouput

        /// <summary>
        /// The database id of the newly created page block. This is 
        /// set after the command has been run.
        /// </summary>
        [OutputValue]
        public int OutputPageBlockId { get; set; }

        #endregion

        #region IValidatableObject

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if ((InsertMode == OrderedItemInsertMode.AfterItem || InsertMode == OrderedItemInsertMode.BeforeItem) 
                && (!AdjacentVersionBlockId.HasValue))
            {
                yield return new ValidationResult("Cannot insert a block before/after when no adjacent block id is provided.");
            }
        }

        #endregion
    }
}
