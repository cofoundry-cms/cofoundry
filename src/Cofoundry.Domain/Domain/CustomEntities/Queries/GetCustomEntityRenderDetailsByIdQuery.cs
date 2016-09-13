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
    public class GetCustomEntityRenderDetailsByIdQuery : IQuery<CustomEntityRenderDetails>, IValidatableObject
    {
        [Required]
        [PositiveInteger]
        public int CustomEntityId { get; set; }

        public WorkFlowStatusQuery WorkFlowStatus { get; set; }

        public int? CustomEntityVersionId { get; set; }

        [Required]
        [PositiveInteger]
        public int PageTemplateId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (WorkFlowStatus == WorkFlowStatusQuery.SpecificVersion && (!CustomEntityVersionId.HasValue || CustomEntityVersionId < 1))
            {
                yield return new ValidationResult("Value cannot be null if WorkFlowStatusQuery.SpecificVersion is specified", new string[] { "CustomEntityVersionId" });
            }
            else if (WorkFlowStatus != WorkFlowStatusQuery.SpecificVersion && CustomEntityVersionId.HasValue)
            {
                yield return new ValidationResult("Value should be null if WorkFlowStatusQuery.SpecificVersion is not specified", new string[] { "CustomEntityVersionId" });
            }
        }

    }
}
