using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to DocumentAssetDetails objects.
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

        /// <summary>
        /// Maps an EF DocumentAsset record from the db into a DocumentAssetDetails 
        /// object. If the db record is null then null is returned.
        /// </summary>
        /// <param name="dbDocument">DocumentAsset record from the database.</param>
        public DocumentAssetSummary Map(DocumentAsset dbDocument)
        {
            if (dbDocument == null) return null;

            var document = new DocumentAssetSummary();
            Map(document, dbDocument);

            return document;
        }

        /// <summary>
        /// Used internally to map a model that inherits from DocumentAssetSummary. 
        /// </summary>
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
}
