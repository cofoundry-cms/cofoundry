using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryCustomEntitySearchQueryBuilder
        : IContentRepositoryCustomEntitySearchQueryBuilder
        , IAdvancedContentRepositoryCustomEntitySearchQueryBuilder
        , IExtendableContentRepositoryPart
    {
        public ContentRepositoryCustomEntitySearchQueryBuilder(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public Task<PagedQueryResult<CustomEntityRenderSummary>> AsRenderSummariesAsync(SearchCustomEntityRenderSummariesQuery query)
        {
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }

        public Task<PagedQueryResult<CustomEntitySummary>> AsSummariesAsync(SearchCustomEntitySummariesQuery query)
        {
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
    }
}
