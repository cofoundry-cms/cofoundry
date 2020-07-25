using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryPageBlockByIdQueryBuilder
        : IAdvancedContentRepositoryPageBlockByIdQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private readonly int _pageVersionBlockId;

        public ContentRepositoryPageBlockByIdQueryBuilder(
            IExtendableContentRepository contentRepository,
            int pageVersionBlockId
            )
        {
            ExtendableContentRepository = contentRepository;
            _pageVersionBlockId = pageVersionBlockId;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IContentRepositoryQueryContext<PageVersionBlockRenderDetails> AsRenderDetails(PublishStatusQuery? publishStatusQuery = null)
        {
            var query = new GetPageVersionBlockRenderDetailsByIdQuery(_pageVersionBlockId, publishStatusQuery);
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
