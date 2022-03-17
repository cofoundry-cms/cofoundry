using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryDocumentAssetByIdRangeQueryBuilder
    : IContentRepositoryDocumentAssetByIdRangeQueryBuilder
    , IExtendableContentRepositoryPart
{
    private readonly IEnumerable<int> _documentAssetIds;

    public ContentRepositoryDocumentAssetByIdRangeQueryBuilder(
        IExtendableContentRepository contentRepository,
        IEnumerable<int> documentAssetIds
        )
    {
        ExtendableContentRepository = contentRepository;
        _documentAssetIds = documentAssetIds;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<IDictionary<int, DocumentAssetRenderDetails>> AsRenderDetails()
    {
        var query = new GetDocumentAssetRenderDetailsByIdRangeQuery(_documentAssetIds);
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
