using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryCustomEntityRoutingRulesRepository
            : IAdvancedContentRepositoryCustomEntityRoutingRulesRepository
            , IExtendableContentRepositoryPart
    {
        public ContentRepositoryCustomEntityRoutingRulesRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        #region queries

        public IAdvancedContentRepositoryGetAllCustomEntityRoutingRuleQueryBuilder GetAll()
        {
            return new ContentRepositoryGetAllCustomEntityRoutingRuleQueryBuilder(ExtendableContentRepository);
        }

        public IAdvancedContentRepositoryCustomEntityRoutingRuleByRouteFormatQueryBuilder GetByRouteFormat(string routeFormat)
        {
            return new ContentRepositoryCustomEntityRoutingRuleByRouteFormatQueryBuilder(ExtendableContentRepository, routeFormat);
        }

        #endregion
    }
}
