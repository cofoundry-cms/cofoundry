using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Finds a document asset by it's database id, returning a 
    /// DocumentAssetDetails projection if it is found, otherwise null.
    /// </summary>
    public class GetDocumentAssetDetailsByIdQuery : IQuery<DocumentAssetDetails>
    {
        public GetDocumentAssetDetailsByIdQuery()
        {
        }

        /// <summary>
        /// Initializes the query with parameters.
        /// </summary>
        /// <param name="documentAssetId">Database id of the document asset to get.</param>
        public GetDocumentAssetDetailsByIdQuery(int documentAssetId)
        {
            DocumentAssetId = documentAssetId;
        }

        /// <summary>
        /// Database id of the document asset to get.
        /// </summary>
        public int DocumentAssetId { get; set; }
    }
}
