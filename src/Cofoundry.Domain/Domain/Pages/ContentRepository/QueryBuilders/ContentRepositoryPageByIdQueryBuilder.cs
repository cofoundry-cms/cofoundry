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

        public IContentRepositoryQueryContext<PageRoute> AsRoute()
        {
            var query = new GetPageRouteByIdQuery(_pageId);
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IContentRepositoryQueryContext<PageRenderSummary> AsRenderSummary()
        {
            var query = new GetPageRenderSummaryByIdQuery(_pageId);
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IContentRepositoryQueryContext<PageRenderSummary> AsRenderSummary(PublishStatusQuery publishStatus)
        {
            var query = new GetPageRenderSummaryByIdQuery(_pageId, publishStatus);
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IContentRepositoryQueryContext<PageRenderSummary> AsRenderSummary(int pageVersionId)
        {
            var query = new GetPageRenderSummaryByIdQuery(_pageId, PublishStatusQuery.SpecificVersion);
            query.PageVersionId = pageVersionId;

            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IContentRepositoryQueryContext<PageRenderDetails> AsRenderDetails()
        {
            var query = new GetPageRenderDetailsByIdQuery(_pageId);
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IContentRepositoryQueryContext<PageRenderDetails> AsRenderDetails(PublishStatusQuery publishStatus)
        {
            var query = new GetPageRenderDetailsByIdQuery(_pageId, publishStatus);
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IContentRepositoryQueryContext<PageRenderDetails> AsRenderDetails(int pageVersionId)
        {
            var query = new GetPageRenderDetailsByIdQuery(_pageId, PublishStatusQuery.SpecificVersion);
            query.PageVersionId = pageVersionId;

            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IContentRepositoryQueryContext<PageDetails> AsDetails()
        {
            var query = new GetPageDetailsByIdQuery(_pageId);
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
