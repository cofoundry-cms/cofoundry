using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public class GetDocumentAssetDetailsByIdQuery : IQuery<DocumentAssetDetails>
    {
        public GetDocumentAssetDetailsByIdQuery()
        {
        }

        public GetDocumentAssetDetailsByIdQuery(int documentAssetId)
        {
            DocumentAssetId = documentAssetId;
        }

        public int DocumentAssetId { get; set; }
    }
}
