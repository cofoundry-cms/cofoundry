﻿using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IDocumentAssetSummaryMapper"/>.
/// </summary>
public class DocumentAssetSummaryMapper : IDocumentAssetSummaryMapper
{
    private readonly IAuditDataMapper _auditDataMapper;
    private readonly IDocumentAssetRouteLibrary _documentAssetRouteLibrary;

    public DocumentAssetSummaryMapper(
        IAuditDataMapper auditDataMapper,
        IDocumentAssetRouteLibrary documentAssetRouteLibrary
        )
    {
        _auditDataMapper = auditDataMapper;
        _documentAssetRouteLibrary = documentAssetRouteLibrary;
    }

    /// <inheritdoc/>
    [return: NotNullIfNotNull(nameof(dbDocument))]
    public DocumentAssetSummary? Map(DocumentAsset? dbDocument)
    {
        if (dbDocument == null)
        {
            return null;
        }

        var document = new DocumentAssetSummary();
        Map(document, dbDocument);

        return document;
    }

    /// <inheritdoc/>
    public DocumentAssetSummary Map<TModel>(TModel document, DocumentAsset dbDocument)
        where TModel : DocumentAssetSummary
    {
        document.AuditData = _auditDataMapper.MapUpdateAuditData(dbDocument);
        document.DocumentAssetId = dbDocument.DocumentAssetId;
        document.FileExtension = dbDocument.FileExtension;
        document.FileName = dbDocument.FileName;
        document.FileSizeInBytes = dbDocument.FileSizeInBytes;
        document.FileStamp = AssetFileStampHelper.ToFileStamp(dbDocument.FileUpdateDate);
        document.Title = dbDocument.Title;
        document.VerificationToken = dbDocument.VerificationToken;

        document.Tags = dbDocument
            .DocumentAssetTags
            .Select(t => t.Tag.TagText)
            .OrderBy(t => t)
            .ToList();

        document.Url = _documentAssetRouteLibrary.DocumentAsset(document);
        document.DownloadUrl = _documentAssetRouteLibrary.DocumentAssetDownload(document);

        return document;
    }
}
