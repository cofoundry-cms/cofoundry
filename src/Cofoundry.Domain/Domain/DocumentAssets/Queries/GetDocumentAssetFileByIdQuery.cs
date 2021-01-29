using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Gets file information about a document asset including
    /// stream access to the file itself.
    /// </summary>
    public class GetDocumentAssetFileByIdQuery : IQuery<DocumentAssetFile>
    {
        public GetDocumentAssetFileByIdQuery()
        {
        }

        /// <summary>
        /// Initializes the query with parameters.
        /// </summary>
        /// <param name="documentAssetId">Database id of the document asset file to get.</param>
        public GetDocumentAssetFileByIdQuery(int documentAssetId)
        {
            DocumentAssetId = documentAssetId;
        }

        /// <summary>
        /// Database id of the document asset file to get.
        /// </summary>
        public int DocumentAssetId { get; set; }
    }
}
