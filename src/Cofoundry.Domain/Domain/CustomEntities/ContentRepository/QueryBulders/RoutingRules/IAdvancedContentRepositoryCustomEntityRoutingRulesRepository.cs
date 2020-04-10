using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries and commands for custom entity routing rules 
    /// definitions (ICustomEntityRoutingRule), which respresent
    /// a dynamic routing rule that is used to work out which custom 
    /// entity should be displayed in a custom entity details page.
    /// </summary>
    public interface IAdvancedContentRepositoryCustomEntityRoutingRulesRepository
    {
        #region queries

        /// <summary>
        /// Queries to return a collection of all ICustomEntityRoutingRule implementations
        /// registered with the DI system.
        /// </summary>
        IAdvancedContentRepositoryGetAllCustomEntityRoutingRuleQueryBuilder GetAll();

        /// <summary>
        /// Finds a custom entity routing rule with a specific route 
        /// format identifier. The routing rules are registered at
        /// startup and will have been checked to ensure there are no
        /// duplicate RouteFormat values.
        /// </summary>
        /// <param name="routeFormat">
        /// The unique identifier for the routing rule to find. This is a
        /// a string representation of the route format e.g. "{Id}/{UrlSlug}"
        /// and will be unique.
        /// </param>
        IAdvancedContentRepositoryCustomEntityRoutingRuleByRouteFormatQueryBuilder GetByRouteFormat(string routeFormat);

        #endregion
    }
}
