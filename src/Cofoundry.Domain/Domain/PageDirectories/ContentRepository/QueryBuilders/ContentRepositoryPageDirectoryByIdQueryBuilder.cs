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

        public Task<PageDirectoryNode> AsNodeAsync()
        {
            var query = new GetPageDirectoryNodeByIdQuery(_pageDirectoryId);
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }

        public Task<PageDirectoryRoute> AsRouteAsync()
        {
            var query = new GetPageDirectoryRouteByIdQuery(_pageDirectoryId);
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
    }
}
