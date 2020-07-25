using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
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

        public IContentRepositoryQueryContext<CustomEntityRoute> AsCustomEntityRoute(GetCustomEntityRouteByPathQuery query)
        {
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
