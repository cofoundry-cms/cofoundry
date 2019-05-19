using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryPageSearchQueryBuilder
        : IContentRepositoryPageSearchQueryBuilder
        , IAdvancedContentRepositoryPageSearchQueryBuilder
        , IExtendableContentRepositoryPart
    {
        public ContentRepositoryPageSearchQueryBuilder(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public Task<PagedQueryResult<PageSummary>> AsSummariesAsync(SearchPageSummariesQuery query)
        {
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }

        public Task<PagedQueryResult<PageRenderSummary>> AsRenderSummariesAsync(SearchPageRenderSummariesQuery query)
        {
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
    }
}
