using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retrieving user data for a unique database id.
    /// </summary>
    public interface IContentRepositoryRewriteRuleByPathQueryBuilder
    {
        /// <summary>
        /// The RewriteRuleSummary projection is small and designed to be cacheable.
        /// This query used a cache by default and is quick to access.
        /// </summary>
        IDomainRepositoryQueryContext<RewriteRuleSummary> AsSummary();
    }
}
