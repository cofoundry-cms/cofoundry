using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetCustomEntityRoutingRuleByRouteFormatQueryHandler
        : IAsyncQueryHandler<GetCustomEntityRoutingRuleByRouteFormatQuery, ICustomEntityRoutingRule>
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
