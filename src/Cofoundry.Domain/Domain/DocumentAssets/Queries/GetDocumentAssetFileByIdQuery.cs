using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public class GetDocumentAssetFileByIdQuery : IQuery<DocumentAssetFile>
    {
        public GetDocumentAssetFileByIdQuery()
        {
        }

        public GetDocumentAssetFileByIdQuery(int documentAssetId)
        {
            DocumentAssetId = documentAssetId;
        }

        public int DocumentAssetId { get; set; }
    }
}
