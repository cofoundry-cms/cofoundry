using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CustomEntityVersionRoute : IVersionRoute
    {
        public CustomEntityVersionRoute()
        {
            AdditionalRoutingData = new Dictionary<string, string>();
        }
        public int VersionId { get; set; }

        public string Title { get; set; }

        public Dictionary<string, string> AdditionalRoutingData { get; set; }

        public DateTime CreateDate { get; set; }

        public WorkFlowStatus WorkFlowStatus { get; set; }
    }
}
