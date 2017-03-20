using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    public class GetPageRenderDetailsByIdQuery : IQuery<PageRenderDetails>, IValidatableObject
    {
        [PositiveInteger]
        [Required]
        public int PageId { get; set; }

        public WorkFlowStatusQuery WorkFlowStatus { get; set; }

        public int? PageVersionId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (WorkFlowStatus == WorkFlowStatusQuery.SpecificVersion && (!PageVersionId.HasValue || PageVersionId < 1))
            {
                yield return new ValidationResult("Value cannot be null if WorkFlowStatusQuery.SpecificVersion is specified", new string[] { nameof(PageVersionId) });
            }
            else if (WorkFlowStatus != WorkFlowStatusQuery.SpecificVersion && PageVersionId.HasValue)
            {
                yield return new ValidationResult("Value should be null if WorkFlowStatusQuery.SpecificVersion is not specified", new string[] { nameof(PageVersionId) });
            }
        }
    }
}
