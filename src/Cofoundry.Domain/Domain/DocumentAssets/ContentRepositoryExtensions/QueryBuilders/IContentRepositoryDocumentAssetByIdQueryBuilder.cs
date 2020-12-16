using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retrieving document asset data for a unique database id.
    /// </summary>
    public interface IContentRepositoryDocumentAssetByIdQueryBuilder
    {
        /// <summary>
        /// The DocumentAssetRenderDetails projection contains all the basic 
        /// information required to render out a document to page, including 
        /// all the data needed to construct a document file url.
        /// </summary>
        IDomainRepositoryQueryContext<DocumentAssetRenderDetails> AsRenderDetails();
    }
}
