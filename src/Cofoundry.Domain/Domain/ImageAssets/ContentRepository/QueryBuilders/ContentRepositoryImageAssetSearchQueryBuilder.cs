using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryImageAssetSearchQueryBuilder
        : IContentRepositoryImageAssetSearchQueryBuilder
        , IExtendableContentRepositoryPart
    {
        public ContentRepositoryImageAssetSearchQueryBuilder(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public Task<PagedQueryResult<ImageAssetSummary>> AsSummariesAsync(SearchImageAssetSummariesQuery query)
        {
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
    }
}
