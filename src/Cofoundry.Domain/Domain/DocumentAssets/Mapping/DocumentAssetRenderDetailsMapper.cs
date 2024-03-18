﻿using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IDocumentAssetRenderDetailsMapper"/>.
/// </summary>
public class DocumentAssetRenderDetailsMapper : IDocumentAssetRenderDetailsMapper
{
    private readonly IDocumentAssetRouteLibrary _documentAssetRouteLibrary;

    public DocumentAssetRenderDetailsMapper(
        IDocumentAssetRouteLibrary documentAssetRouteLibrary
        )
    {
        _documentAssetRouteLibrary = documentAssetRouteLibrary;
    }

    /// <inheritdoc/>
    [return: NotNullIfNotNull(nameof(dbDocument))]
    public DocumentAssetRenderDetails? Map(DocumentAsset? dbDocument)
    {
        if (dbDocument == null)
        {
            return null;
        }

        var document = new DocumentAssetRenderDetails();

        document.DocumentAssetId = dbDocument.DocumentAssetId;
        document.FileExtension = dbDocument.FileExtension;
        document.FileName = dbDocument.FileName;
        document.FileSizeInBytes = dbDocument.FileSizeInBytes;
        document.Title = dbDocument.Title;
        document.FileStamp = AssetFileStampHelper.ToFileStamp(dbDocument.FileUpdateDate);
        document.Description = dbDocument.Description;
        document.VerificationToken = dbDocument.VerificationToken;

        document.Url = _documentAssetRouteLibrary.DocumentAsset(document);
        document.DownloadUrl = _documentAssetRouteLibrary.DocumentAssetDownload(document);

        return document;
    }
}
