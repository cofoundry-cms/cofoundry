using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IDocumentAssetDetailsMapper"/>.
/// </summary>
public class DocumentAssetDetailsMapper : IDocumentAssetDetailsMapper
{
    private readonly DocumentAssetSummaryMapper _documentAssetSummaryMapper;

    public DocumentAssetDetailsMapper(
        IAuditDataMapper auditDataMapper,
        IDocumentAssetRouteLibrary documentAssetRouteLibrary
        )
    {
        _documentAssetSummaryMapper = new DocumentAssetSummaryMapper(auditDataMapper, documentAssetRouteLibrary);
    }

    /// <inheritdoc/>
    [return: NotNullIfNotNull(nameof(dbDocument))]
    public DocumentAssetDetails? Map(DocumentAsset? dbDocument)
    {
        if (dbDocument == null)
        {
            return null;
        }

        var document = new DocumentAssetDetails();
        _documentAssetSummaryMapper.Map(document, dbDocument);
        document.Description = dbDocument.Description;
        document.FileUpdateDate = dbDocument.FileUpdateDate;

        return document;
    }
}
