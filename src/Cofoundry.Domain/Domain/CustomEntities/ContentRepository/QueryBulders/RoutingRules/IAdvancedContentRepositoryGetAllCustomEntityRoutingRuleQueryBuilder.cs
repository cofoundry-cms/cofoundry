using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries to return a collection of all ICustomEntityRoutingRule implementations
    /// registered with the DI system.
    /// </summary>
    public interface IAdvancedContentRepositoryGetAllCustomEntityRoutingRuleQueryBuilder
    {
        /// <summary>
        /// Returns all instances of ICustomEntityRoutingRule registered
        /// in the application.
        /// </summary>
        IDomainRepositoryQueryContext<ICollection<ICustomEntityRoutingRule>> AsRoutingRules();
    }
}
