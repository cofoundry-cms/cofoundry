using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries to find custom entity routing rules, which are used to determine
    /// which custom entity should be displayed in a custom entity details page.
    /// </summary>
    public interface IAdvancedContentRepositoryCustomEntityRoutingRuleByRouteFormatQueryBuilder
    {
        /// <summary>
        /// Returns a single routing rule that matches the route format exactly, 
        /// or null if one cannot be found.
        /// </summary>
        IDomainRepositoryQueryContext<ICustomEntityRoutingRule> AsRoutingRule();
    }
}
