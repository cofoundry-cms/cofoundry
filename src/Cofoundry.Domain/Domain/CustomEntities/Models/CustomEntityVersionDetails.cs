using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CustomEntityVersionDetails
    {
        public int CustomEntityVersionId { get; set; }

        public string Title { get; set; }

        public WorkFlowStatus WorkFlowStatus { get; set; }

        public ICustomEntityDataModel Model { get; set; }

        public ICollection<CustomEntityPage> Pages { get; set; }

        public CreateAuditData AuditData { get; set; }
    }
}
