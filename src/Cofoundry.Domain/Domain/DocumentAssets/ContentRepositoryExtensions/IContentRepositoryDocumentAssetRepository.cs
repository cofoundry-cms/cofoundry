using System.Collections.Generic;

namespace Cofoundry.Domain
{
    /// <summary>
    /// IContentRespository extension root for the DocumentAsset entity.
    /// </summary>
    public interface IContentRepositoryDocumentAssetRepository
    {
        #region queries

        /// <summary>
        /// Retrieve an document asset by a unique database id.
        /// </summary>
        /// <param name="documentAssetId">DocumentAssetId of the document to get.</param>
        IContentRepositoryDocumentAssetByIdQueryBuilder GetById(int documentAssetId);

        /// <summary>
        /// Retrieve a set of document assets using a batch of database ids.
        /// The Cofoundry.Core dictionary extensions can be useful for 
        /// ordering the results e.g. results.FilterAndOrderByKeys(ids).
        /// </summary>
        /// <param name="documentAssetIds">Range of DocumentAssetIds of the documents to get.</param>
        IContentRepositoryDocumentAssetByIdRangeQueryBuilder GetByIdRange(IEnumerable<int> documentAssetIds);

        /// <summary>
        /// Search for document assets, returning paged lists of data.
        /// </summary>
        IContentRepositoryDocumentAssetSearchQueryBuilder Search();

        #endregion
    }
}
