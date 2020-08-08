using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryPageByIdRangeQueryBuilder
        : IContentRepositoryPageByIdRangeQueryBuilder
        , IAdvancedContentRepositoryPageByIdRangeQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private readonly IEnumerable<int> _pageIds;

        public ContentRepositoryPageByIdRangeQueryBuilder(
            IExtendableContentRepository contentRepository,
            IEnumerable<int> pageIds
            )
        {
            ExtendableContentRepository = contentRepository;
            _pageIds = pageIds;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IDomainRepositoryQueryContext<IDictionary<int, PageRoute>> AsRoutes()
        {
            var query = new GetPageRoutesByIdRangeQuery(_pageIds);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
        
        public IDomainRepositoryQueryContext<IDictionary<int, PageRenderSummary>> AsRenderSummaries(PublishStatusQuery? publishStatus = null)
        {
            var query = new GetPageRenderSummariesByIdRangeQuery(_pageIds, publishStatus);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<IDictionary<int, PageRenderDetails>> AsRenderDetails(PublishStatusQuery? publishStatus = null)
        {
            var query = new GetPageRenderDetailsByIdRangeQuery(_pageIds, publishStatus);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<IDictionary<int, PageSummary>> AsSummaries()
        {
            var query = new GetPageSummariesByIdRangeQuery(_pageIds);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
