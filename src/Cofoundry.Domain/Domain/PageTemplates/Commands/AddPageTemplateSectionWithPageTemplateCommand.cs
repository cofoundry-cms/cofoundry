using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class AddPageTemplateSectionWithPageTemplateCommand : ICommand, ILoggableCommand, IValidatableObject
    {
        [Required]
        public string Name { get; set; }

        public bool PermitAllModuleTypes { get; set; }

        public int[] PermittedModuleTypeIds { get; set; }

        public bool IsCustomEntitySection { get; set; }

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
