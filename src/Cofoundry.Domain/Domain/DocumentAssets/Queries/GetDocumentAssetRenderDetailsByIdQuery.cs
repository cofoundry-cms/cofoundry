using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Finds a document asset by it's database id, returning a 
    /// DocumentAssetRenderDetails projection if it is found, otherwise 
    /// null. A DocumentAssetRenderDetails contains all the basic information 
    /// required to render out a document to page, including all the data 
    /// needed to construct a document file url.
    /// </summary>
    public class GetDocumentAssetRenderDetailsByIdQuery : IQuery<DocumentAssetRenderDetails>
    {
        public GetDocumentAssetRenderDetailsByIdQuery()
        {
        }

        /// <summary>
        /// Initializes the query with parameters.
        /// </summary>
        /// <param name="documentAssetId">Database id of the document asset to get.</param>
        public GetDocumentAssetRenderDetailsByIdQuery(int documentAssetId)
        {
            DocumentAssetId = documentAssetId;
        }

        /// <summary>
        /// Database id of the document asset to get.
        /// </summary>
        public int DocumentAssetId { get; set; }
    }
}
