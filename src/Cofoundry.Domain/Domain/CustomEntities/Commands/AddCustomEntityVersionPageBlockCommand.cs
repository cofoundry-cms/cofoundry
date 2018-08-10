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
    public class AddCustomEntityVersionPageBlockCommand : ICommand, ILoggableCommand, IValidatableObject, IPageVersionBlockDataModelCommand
    {
        [Required]
        [PositiveInteger]
        public int CustomEntityVersionId { get; set; }

        [Required]
        [PositiveInteger]
        public int PageId { get; set; }

        [Required]
        [PositiveInteger]
        public int PageBlockTypeId { get; set; }

        [PositiveInteger]
        public int? PageBlockTypeTemplateId { get; set; }

        [Required]
        [PositiveInteger]
        public int PageTemplateRegionId { get; set; }

        /// <summary>
        /// Where to place the item in the collection
        /// </summary>
        public OrderedItemInsertMode InsertMode { get; set; }

        [PositiveInteger]
        public int? AdjacentVersionBlockId { get; set; }

        [Required]
        [ValidateObject]
        public IPageBlockTypeDataModel DataModel { get; set; }

        #region Ouput

        [OutputValue]
        public int OutputCustomEntityVersionPageBlockId { get; set; }

        #endregion

        #region IValidatableObject

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if ((InsertMode == OrderedItemInsertMode.AfterItem || InsertMode == OrderedItemInsertMode.BeforeItem) 
                && (!AdjacentVersionBlockId.HasValue))
            {
                yield return new ValidationResult("Cannot insert a page block before/after when no adjacent page block is provided.");
            }
        }

        #endregion
    }
}
