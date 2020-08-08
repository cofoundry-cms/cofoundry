using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryGetAllCustomEntityRoutingRuleQueryBuilder
        : IAdvancedContentRepositoryGetAllCustomEntityRoutingRuleQueryBuilder
        , IExtendableContentRepositoryPart
    {
        public ContentRepositoryGetAllCustomEntityRoutingRuleQueryBuilder(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IDomainRepositoryQueryContext<ICollection<ICustomEntityRoutingRule>> AsRoutingRules()
        {
            var query = new GetAllCustomEntityRoutingRulesQuery();
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
