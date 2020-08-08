using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Query to return a collection of all ICustomEntityRoutingRule implementations
    /// registered with the DI system. This query checks the validity of the 
    /// collection before returning them.
    /// </summary>
    public class GetAllCustomEntityRoutingRulesQueryHandler 
        : IQueryHandler<GetAllCustomEntityRoutingRulesQuery, ICollection<ICustomEntityRoutingRule>>
        , IIgnorePermissionCheckHandler
    {
        private readonly IEnumerable<ICustomEntityRoutingRule> _customEntityRoutingRules;

        public GetAllCustomEntityRoutingRulesQueryHandler(
            IEnumerable<ICustomEntityRoutingRule> customEntityRoutingRules
            )
        {
            _customEntityRoutingRules = customEntityRoutingRules;
        }

        public Task<ICollection<ICustomEntityRoutingRule>> ExecuteAsync(GetAllCustomEntityRoutingRulesQuery query, IExecutionContext executionContext)
        {
            var duplicateRule = _customEntityRoutingRules
                .GroupBy(r => r.RouteFormat)
                .FirstOrDefault(g => g.Count() > 1);

            if (duplicateRule != null)
            {
                throw new Exception("Multiple handlers cannot exist using the same RouteFormat. Duplicate: " + duplicateRule.Key);
            }

            return Task.FromResult<ICollection<ICustomEntityRoutingRule>>(_customEntityRoutingRules.ToList());
        }
    }
}
