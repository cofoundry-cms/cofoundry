using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryDocumentAssetSearchQueryBuilder
        : IContentRepositoryDocumentAssetSearchQueryBuilder
        , IExtendableContentRepositoryPart
    {
        public ContentRepositoryDocumentAssetSearchQueryBuilder(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public Task<PagedQueryResult<DocumentAssetSummary>> AsSummariesAsync(SearchDocumentAssetSummariesQuery query)
        {
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
    }
}
