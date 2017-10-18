using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple mapper for mapping to DocumentAssetRenderDetails objects.
    /// </summary>
    public class DocumentAssetRenderDetailsMapper : IDocumentAssetRenderDetailsMapper
    {
        /// <summary>
        /// Maps an EF DocumentAsset record from the db into a DocumentAssetDetails 
        /// object. If the db record is null then null is returned.
        /// </summary>
        /// <param name="dbDocument">DocumentAsset record from the database.</param>
        public DocumentAssetRenderDetails Map(DocumentAsset dbDocument)
        {
            if (dbDocument == null) return null;

            var document = new DocumentAssetRenderDetails();

            document.DocumentAssetId = dbDocument.DocumentAssetId;
            document.FileExtension = dbDocument.FileExtension;
            document.FileName = dbDocument.FileName;
            document.FileSizeInBytes = dbDocument.FileSizeInBytes;
            document.Title = dbDocument.Title;
            document.Description = dbDocument.Description;

            return document;
        }
    }
}
