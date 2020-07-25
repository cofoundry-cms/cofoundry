using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryPageDirectoryByIdQueryBuilder
        : IAdvancedContentRepositoryPageDirectoryByIdQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private readonly int _pageDirectoryId;

        public ContentRepositoryPageDirectoryByIdQueryBuilder(
            IExtendableContentRepository contentRepository,
            int pageDirectoryId
            )
        {
            ExtendableContentRepository = contentRepository;
            _pageDirectoryId = pageDirectoryId;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IContentRepositoryQueryContext<PageDirectoryNode> AsNode()
        {
            var query = new GetPageDirectoryNodeByIdQuery(_pageDirectoryId);
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IContentRepositoryQueryContext<PageDirectoryRoute> AsRoute()
        {
            var query = new GetPageDirectoryRouteByIdQuery(_pageDirectoryId);
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
