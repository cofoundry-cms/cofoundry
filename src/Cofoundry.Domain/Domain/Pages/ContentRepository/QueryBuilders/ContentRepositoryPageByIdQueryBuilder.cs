using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryPageByIdQueryBuilder
        : IContentRepositoryPageByIdQueryBuilder
        , IAdvancedContentRepositoryPageByIdQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private readonly int _pageId;

        public ContentRepositoryPageByIdQueryBuilder(
            IExtendableContentRepository contentRepository,
            int pageId
            )
        {
            ExtendableContentRepository = contentRepository;
            _pageId = pageId;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public Task<PageRoute> AsRouteAsync()
        {
            var query = new GetPageRouteByIdQuery(_pageId);
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }

        public Task<PageRenderSummary> AsRenderSummaryAsync(PublishStatusQuery? publishStatus = null)
        {
            var query = new GetPageRenderSummaryByIdQuery(_pageId, publishStatus);
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }

        public Task<PageRenderSummary> AsRenderSummaryAsync(int pageVersionId)
        {
            var query = new GetPageRenderSummaryByIdQuery(_pageId, PublishStatusQuery.SpecificVersion);
            query.PageVersionId = pageVersionId;

            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }

        public Task<PageRenderDetails> AsRenderDetailsAsync(PublishStatusQuery? publishStatus = null)
        {
            var query = new GetPageRenderDetailsByIdQuery(_pageId, publishStatus);
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }

        public Task<PageRenderDetails> AsRenderDetailsAsync(int pageVersionId)
        {
            var query = new GetPageRenderDetailsByIdQuery(_pageId, PublishStatusQuery.SpecificVersion);
            query.PageVersionId = pageVersionId;

            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }

        public Task<PageDetails> AsDetailsAsync()
        {
            var query = new GetPageDetailsByIdQuery(_pageId);
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
    }
}
