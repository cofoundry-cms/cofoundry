using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CustomEntityRoutingParameters
    {
        public CustomEntityRoutingParameters()
        {
            AdditionalValues = new Dictionary<string, string>();
        }

        public int? CustomEntityId { get; set; }

        public string UrlSlug { get; set; }

        public Dictionary<string, string> AdditionalValues { get; set; }
    }
}
