using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryPageNotFoundQueryBuilder
        : IAdvancedContentRepositoryPageNotFoundQueryBuilder
        , IExtendableContentRepositoryPart
    {
        public ContentRepositoryPageNotFoundQueryBuilder(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IDomainRepositoryQueryContext<PageRoute> GetByPath(GetNotFoundPageRouteByPathQuery query)
        {
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
