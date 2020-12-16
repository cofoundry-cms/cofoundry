using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
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

        public IDomainRepositoryQueryContext<ICustomEntityRoutingRule> AsRoutingRule()
        {
            var query = new GetCustomEntityRoutingRuleByRouteFormatQuery(_routeFormat);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
