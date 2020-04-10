using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryCustomEntityRoutingRuleByRouteFormatQueryBuilder
        : IAdvancedContentRepositoryCustomEntityRoutingRuleByRouteFormatQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private readonly string _routeFormat;

        public ContentRepositoryCustomEntityRoutingRuleByRouteFormatQueryBuilder(
            IExtendableContentRepository contentRepository,
            string routeFormat
            )
        {
            ExtendableContentRepository = contentRepository;
            _routeFormat = routeFormat;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public Task<ICustomEntityRoutingRule> AsRoutingRuleAsync()
        {
            var query = new GetCustomEntityRoutingRuleByRouteFormatQuery(_routeFormat);
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
    }
}
