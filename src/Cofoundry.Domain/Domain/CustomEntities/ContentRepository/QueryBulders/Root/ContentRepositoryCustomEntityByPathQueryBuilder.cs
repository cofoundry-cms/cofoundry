using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryCustomEntityByPathQueryBuilder
        : IAdvancedContentRepositoryCustomEntityByPathQueryBuilder
        , IExtendableContentRepositoryPart
    {
        public ContentRepositoryCustomEntityByPathQueryBuilder(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IDomainRepositoryQueryContext<CustomEntityRoute> AsCustomEntityRoute(GetCustomEntityRouteByPathQuery query)
        {
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
