using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CustomEntityVersionSummary : ICreateAudited
    {
        public int CustomEntityVersionId { get; set; }

        public string Title { get; set; }
                
        public WorkFlowStatus WorkFlowStatus { get; set; }
        
        public CreateAuditData AuditData { get; set; }
    }
}
