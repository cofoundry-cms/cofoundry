using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetCustomEntityRenderSummaryByDefinitionCodeQuery : IQuery<IEnumerable<CustomEntityRenderSummary>>
    {
        public GetCustomEntityRenderSummaryByDefinitionCodeQuery()
        {
        }

        public GetCustomEntityRenderSummaryByDefinitionCodeQuery(
            string customEntityDefinitionCode,
            WorkFlowStatusQuery workflowStatus = WorkFlowStatusQuery.Latest
            )
        {
            CustomEntityDefinitionCode = customEntityDefinitionCode;
            WorkFlowStatus = workflowStatus;
        }

        [Required]
        public string CustomEntityDefinitionCode { get; set; }

        public WorkFlowStatusQuery WorkFlowStatus { get; set; }
    }
}
