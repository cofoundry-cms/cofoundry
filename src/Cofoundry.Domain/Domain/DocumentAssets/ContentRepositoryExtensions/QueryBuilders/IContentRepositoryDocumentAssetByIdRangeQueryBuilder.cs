using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retrieving a set of document assets using a batch of 
    /// database ids. The Cofoundry.Core dictionary extensions can be 
    /// useful for ordering the results e.g. results.FilterAndOrderByKeys(ids).
    /// </summary>
    public interface IContentRepositoryDocumentAssetByIdRangeQueryBuilder
    {
        /// <summary>
        /// The DocumentAssetRenderDetails projection contains all the basic 
        /// information required to render out a document to page, including 
        /// all the data needed to construct a document file url.
        /// </summary>
        IDomainRepositoryQueryContext<IDictionary<int, DocumentAssetRenderDetails>> AsRenderDetails();
    }
}
