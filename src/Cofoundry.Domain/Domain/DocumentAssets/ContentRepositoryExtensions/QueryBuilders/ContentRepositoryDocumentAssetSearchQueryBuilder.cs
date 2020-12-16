using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
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

        public IDomainRepositoryQueryContext<PagedQueryResult<DocumentAssetSummary>> AsSummaries(SearchDocumentAssetSummariesQuery query)
        {
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
