using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class RewriteRuleSummary
    {
        public int RewriteRuleId { get; set; }
        public string WriteFrom { get; set; }
        public string WriteTo { get; set; }
    }
}
