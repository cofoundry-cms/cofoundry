using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    public class AddPageTemplateSectionCommand : ICommand, ILoggableCommand, IValidatableObject
    {
        [Required]
        [PositiveInteger]
        public int PageTemplateId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public bool IsCustomEntitySection { get; set; }

        public bool PermitAllModuleTypes { get; set; }

        public int[] PermittedModuleTypeIds { get; set; }

        [OutputValue]
        public int OutputPageTemplateSectionId { get; set; }

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
