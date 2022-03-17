using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryDocumentAssetByIdQueryBuilder
    : IContentRepositoryDocumentAssetByIdQueryBuilder
    , IExtendableContentRepositoryPart
{
    private readonly int _documentAssetId;

    public ContentRepositoryDocumentAssetByIdQueryBuilder(
        IExtendableContentRepository contentRepository,
        int documentAssetId
        )
    {
        ExtendableContentRepository = contentRepository;
        _documentAssetId = documentAssetId;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<DocumentAssetRenderDetails> AsRenderDetails()
    {
        var query = new GetDocumentAssetRenderDetailsByIdQuery(_documentAssetId);
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
