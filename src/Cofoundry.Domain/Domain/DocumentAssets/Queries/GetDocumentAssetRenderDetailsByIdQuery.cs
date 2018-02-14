using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public class GetDocumentAssetRenderDetailsByIdQuery : IQuery<DocumentAssetRenderDetails>
    {
        public GetDocumentAssetRenderDetailsByIdQuery()
        {
        }

        public GetDocumentAssetRenderDetailsByIdQuery(int documentAssetId)
        {
            DocumentAssetId = documentAssetId;
        }

        public int DocumentAssetId { get; set; }
    }
}
