using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

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
