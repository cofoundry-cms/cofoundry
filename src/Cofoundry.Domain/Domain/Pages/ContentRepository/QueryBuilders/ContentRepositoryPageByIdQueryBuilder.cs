using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
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

        public IDomainRepositoryQueryContext<PageRoute> AsRoute()
        {
            var query = new GetPageRouteByIdQuery(_pageId);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<PageRenderSummary> AsRenderSummary()
        {
            var query = new GetPageRenderSummaryByIdQuery(_pageId);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<PageRenderSummary> AsRenderSummary(PublishStatusQuery publishStatus)
        {
            var query = new GetPageRenderSummaryByIdQuery(_pageId, publishStatus);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<PageRenderSummary> AsRenderSummary(int pageVersionId)
        {
            var query = new GetPageRenderSummaryByIdQuery(_pageId, PublishStatusQuery.SpecificVersion);
            query.PageVersionId = pageVersionId;

            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<PageRenderDetails> AsRenderDetails()
        {
            var query = new GetPageRenderDetailsByIdQuery(_pageId);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<PageRenderDetails> AsRenderDetails(PublishStatusQuery publishStatus)
        {
            var query = new GetPageRenderDetailsByIdQuery(_pageId, publishStatus);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<PageRenderDetails> AsRenderDetails(int pageVersionId)
        {
            var query = new GetPageRenderDetailsByIdQuery(_pageId, PublishStatusQuery.SpecificVersion);
            query.PageVersionId = pageVersionId;

            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<PageDetails> AsDetails()
        {
            var query = new GetPageDetailsByIdQuery(_pageId);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
