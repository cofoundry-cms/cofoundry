using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Finds a custom entity routing rule with a specific route 
    /// format identifier. The routing rules are registered at
    /// startup and will have been checked to ensure there are no
    /// duplicate RouteFormat values.
    /// </summary>
    public class GetCustomEntityRoutingRuleByRouteFormatQueryHandler
        : IQueryHandler<GetCustomEntityRoutingRuleByRouteFormatQuery, ICustomEntityRoutingRule>
        , IIgnorePermissionCheckHandler
    {
        private readonly IEnumerable<ICustomEntityRoutingRule> _customEntityRoutingRules;

        public GetCustomEntityRoutingRuleByRouteFormatQueryHandler(
            IEnumerable<ICustomEntityRoutingRule> customEntityRoutingRules
            )
        {
            _customEntityRoutingRules = customEntityRoutingRules;
        }

        public Task<ICustomEntityRoutingRule> ExecuteAsync(GetCustomEntityRoutingRuleByRouteFormatQuery query, IExecutionContext executionContext)
        {
            var routingRule = _customEntityRoutingRules.SingleOrDefault(r => r.RouteFormat == query.RouteFormat);

            return Task.FromResult(routingRule);
        }
    }
}
