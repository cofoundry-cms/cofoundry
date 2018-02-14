using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Gets a rewrite rule that matches the specified path in the 
    /// 'WriteFrom' property. If multiple matches are found, the most
    /// recently added rule is returned. Non-file paths are matched with
    /// and without the trailing slash.
    /// </summary>
    public class GetRewriteRuleSummaryByPathQuery : IQuery<RewriteRuleSummary>
    {
        /// <summary>
        /// Path to check for a rewrite rule. For non-file paths the trailing slash 
        /// is optional. Also supports '*' wildcard matching at the end of the path.
        /// </summary>
        [Required]
        public string Path { get; set; }
    }
}
