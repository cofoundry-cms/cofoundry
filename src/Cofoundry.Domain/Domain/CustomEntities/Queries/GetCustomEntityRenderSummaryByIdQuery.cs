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
    public class GetCustomEntityRenderSummaryByIdQuery : IQuery<CustomEntityRenderSummary>
    {
        [Required]
        [PositiveInteger]
        public int CustomEntityId { get; set; }

        public WorkFlowStatusQuery WorkFlowStatus { get; set; }
    }
}
