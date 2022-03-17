using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <inheritdoc/>
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

    /// <summary>
    /// Maps an EF DocumentAsset record from the db into a DocumentAssetDetails 
    /// object. If the db record is null then null is returned.
    /// </summary>
    /// <param name="dbDocument">DocumentAsset record from the database.</param>
    public DocumentAssetDetails Map(DocumentAsset dbDocument)
    {
        if (dbDocument == null) return null;

        var document = new DocumentAssetDetails();
        _documentAssetSummaryMapper.Map(document, dbDocument);
        document.Description = dbDocument.Description;
        document.FileUpdateDate = dbDocument.FileUpdateDate;

        return document;
    }
}
