using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetCustomEntityRenderSummariesByDefinitionCodeQuery : IQuery<ICollection<CustomEntityRenderSummary>>
    {
        public GetCustomEntityRenderSummariesByDefinitionCodeQuery()
        {
        }

        public GetCustomEntityRenderSummariesByDefinitionCodeQuery(
            string customEntityDefinitionCode,
            PublishStatusQuery workflowStatus = PublishStatusQuery.Published
            )
        {
            CustomEntityDefinitionCode = customEntityDefinitionCode;
            PublishStatus = workflowStatus;
        }

        [Required]
        public string CustomEntityDefinitionCode { get; set; }

        public PublishStatusQuery PublishStatus { get; set; }
    }
}
