using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetCustomEntityRoutingRuleByRouteFormatQueryHandler
        : IQueryHandler<GetCustomEntityRoutingRuleByRouteFormatQuery, ICustomEntityRoutingRule>
        , IAsyncQueryHandler<GetCustomEntityRoutingRuleByRouteFormatQuery, ICustomEntityRoutingRule>
        , IIgnorePermissionCheckHandler
    {
        private readonly ICustomEntityRoutingRule[] _customEntityRoutingRules;

        public GetCustomEntityRoutingRuleByRouteFormatQueryHandler(
            ICustomEntityRoutingRule[] customEntityRoutingRules
            )
        {
            _customEntityRoutingRules = customEntityRoutingRules;
        }

        public ICustomEntityRoutingRule Execute(GetCustomEntityRoutingRuleByRouteFormatQuery query, IExecutionContext executionContext)
        {
            var routingRule = _customEntityRoutingRules.SingleOrDefault(r => r.RouteFormat == query.RouteFormat);

            return routingRule;
        }

        public Task<ICustomEntityRoutingRule> ExecuteAsync(GetCustomEntityRoutingRuleByRouteFormatQuery query, IExecutionContext executionContext)
        {
            var rules = Execute(query, executionContext);

            return Task.FromResult(rules);
        }
    }
}
