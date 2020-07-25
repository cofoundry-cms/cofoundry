using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryPageNotFoundQueryBuilder
        : IAdvancedContentRepositoryPageNotFoundQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private readonly int _pageId;

        public ContentRepositoryPageNotFoundQueryBuilder(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IContentRepositoryQueryContext<PageRoute> GetByPath(GetNotFoundPageRouteByPathQuery query)
        {
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
