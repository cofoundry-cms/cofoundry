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
    public class GetCustomEntityRenderSummaryByIdQuery : IQuery<CustomEntityRenderSummary>, IValidatableObject
    {
        public GetCustomEntityRenderSummaryByIdQuery() { }

        public GetCustomEntityRenderSummaryByIdQuery(int customEntityId, PublishStatusQuery workFlowStatus = PublishStatusQuery.Published)
        {
            CustomEntityId = customEntityId;
            PublishStatus = workFlowStatus;
        }

        [Required]
        [PositiveInteger]
        public int CustomEntityId { get; set; }

        /// <summary>
        /// Use this to specify a specific version to return in the query. Mandatory when using PublishStatusQuery.SpecificVersion
        /// </summary>
        public PublishStatusQuery PublishStatus { get; set; }

        /// <summary>
        /// Use this to specify a specific version to return in the query. Mandatory when using PublishStatusQuery.SpecificVersion
        /// </summary>
        public int? CustomEntityVersionId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return PublishStatusQueryModelValidator.ValidateVersionId(PublishStatus, CustomEntityVersionId, nameof(CustomEntityVersionId));
        }
    }
}
