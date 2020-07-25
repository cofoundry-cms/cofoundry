using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
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

        public IContentRepositoryQueryContext<IDictionary<int, PageRoute>> AsRoutes()
        {
            var query = new GetPageRoutesByIdRangeQuery(_pageIds);
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
        
        public IContentRepositoryQueryContext<IDictionary<int, PageRenderSummary>> AsRenderSummaries(PublishStatusQuery? publishStatus = null)
        {
            var query = new GetPageRenderSummariesByIdRangeQuery(_pageIds, publishStatus);
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IContentRepositoryQueryContext<IDictionary<int, PageRenderDetails>> AsRenderDetails(PublishStatusQuery? publishStatus = null)
        {
            var query = new GetPageRenderDetailsByIdRangeQuery(_pageIds, publishStatus);
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IContentRepositoryQueryContext<IDictionary<int, PageSummary>> AsSummaries()
        {
            var query = new GetPageSummariesByIdRangeQuery(_pageIds);
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
