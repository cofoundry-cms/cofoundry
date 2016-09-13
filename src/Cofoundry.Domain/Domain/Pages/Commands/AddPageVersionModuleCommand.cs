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
    public class AddPageVersionModuleCommand : ICommand, ILoggableCommand, IValidatableObject, IPageVersionModuleDataModelCommand
    {
        [Required]
        [PositiveInteger]
        public int PageVersionId { get; set; }

        [Required]
        [PositiveInteger]
        public int PageModuleTypeId { get; set; }

        [PositiveInteger]
        public int? PageModuleTypeTemplateId { get; set; }

        [Required]
        [PositiveInteger]
        public int PageTemplateSectionId { get; set; }

        /// <summary>
        /// Where to place the item in the collection
        /// </summary>
        public OrderedItemInsertMode InsertMode { get; set; }

        [PositiveInteger]
        public int? AdjacentVersionModuleId { get; set; }

        [Required]
        [ValidateObject]
        public IPageModuleDataModel DataModel { get; set; }

        #region Ouput

        [OutputValue]
        public int OutputPageModuleId { get; set; }

        #endregion

        #region IValidatableObject

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if ((InsertMode == OrderedItemInsertMode.AfterItem || InsertMode == OrderedItemInsertMode.BeforeItem) 
                && (!AdjacentVersionModuleId.HasValue))
            {
                yield return new ValidationResult("Cannot insert a module before/after when no adjacent module is provided.");
            }
        }

        #endregion
    }
}
