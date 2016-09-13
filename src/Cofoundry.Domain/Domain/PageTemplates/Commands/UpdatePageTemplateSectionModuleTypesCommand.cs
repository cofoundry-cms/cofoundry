using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    public class UpdatePageTemplateSectionModuleTypesCommand : ICommand, ILoggableCommand, IValidatableObject
    {
        [Required]
        [PositiveInteger]
        public int PageTemplateSectionId { get; set; }

        public bool PermitAllModuleTypes { get; set; }

        public int[] PermittedModuleTypeIds { get; set; }

        #region custom validation

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!PermitAllModuleTypes && (PermittedModuleTypeIds == null || !PermittedModuleTypeIds.Any()))
            {
                yield return new ValidationResult("No modules selected", new[] { "PermittedModuleTypeIds" });
            }
        }

        #endregion
    }
}
