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

        public Task<IDictionary<int, PageRoute>> AsRoutesAsync()
        {
            var query = new GetPageRoutesByIdRangeQuery(_pageIds);
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
        
        public Task<IDictionary<int, PageRenderSummary>> AsRenderSummariesAsync(PublishStatusQuery? publishStatus = null)
        {
            var query = new GetPageRenderSummariesByIdRangeQuery(_pageIds, publishStatus);
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }

        public Task<IDictionary<int, PageRenderDetails>> AsRenderDetailsAsync(PublishStatusQuery? publishStatus = null)
        {
            var query = new GetPageRenderDetailsByIdRangeQuery(_pageIds, publishStatus);
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }

        public Task<IDictionary<int, PageSummary>> AsSummariesAsync()
        {
            var query = new GetPageSummariesByIdRangeQuery(_pageIds);
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
    }
}
