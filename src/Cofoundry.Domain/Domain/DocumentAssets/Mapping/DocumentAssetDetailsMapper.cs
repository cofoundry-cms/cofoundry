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
    public class DocumentAssetDetailsMapper : IDocumentAssetDetailsMapper
    {
        private readonly DocumentAssetSummaryMapper _documentAssetSummaryMapper;
        private readonly IDocumentAssetRouteLibrary _documentAssetRouteLibrary;
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
            document.FileUpdateDate = DbDateTimeMapper.AsUtc(dbDocument.FileUpdateDate);

            return document;
        }
    }
}
