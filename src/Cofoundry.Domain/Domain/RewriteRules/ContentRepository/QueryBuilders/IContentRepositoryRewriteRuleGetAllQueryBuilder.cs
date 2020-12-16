using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries to return all rewrite rules.
    /// </summary>
    public interface IContentRepositoryRewriteRuleGetAllQueryBuilder
    {
        /// <summary>
        /// The RewriteRuleSummary projection is small and designed to be cacheable.
        /// This result set is cached by default and is quick to access.
        /// </summary>
        IDomainRepositoryQueryContext<ICollection<RewriteRuleSummary>> AsSummaries();
    }
}
