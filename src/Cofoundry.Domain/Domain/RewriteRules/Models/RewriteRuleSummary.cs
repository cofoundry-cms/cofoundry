using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Rewrite rules is be used to redirect users from one url to another.
    /// This projection is small, designed to be cacheable and is used in
    /// the routing system.
    /// </summary>
    public class RewriteRuleSummary
    {
        /// <summary>
        /// Identifier and database primary key.
        /// </summary>
        public int RewriteRuleId { get; set; }

        /// <summary>
        /// The incoming url to redirect from. Wildcard matching is supported
        /// by using an asterisk '*' at the end of the path.
        /// </summary>
        public string WriteFrom { get; set; }

        /// <summary>
        /// The url to rewrite to.
        /// </summary>
        public string WriteTo { get; set; }
    }
}
