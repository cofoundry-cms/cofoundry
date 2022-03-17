namespace Cofoundry.Domain;

/// <summary>
/// IAdvancedContentRespository extension root for the DocumentAsset entity.
/// </summary>
public interface IAdvancedContentRepositoryDocumentAssetRepository
{
    /// <summary>
    /// Retrieve an document asset by a unique database id.
    /// </summary>
    /// <param name="documentAssetId">DocumentAssetId of the document to get.</param>
    IAdvancedContentRepositoryDocumentAssetByIdQueryBuilder GetById(int documentAssetId);

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

    /// <summary>
    /// Adds a new document asset.
    /// </summary>
    /// <param name="command">Command parameters.</param>
    /// <returns>Id of the newly created document asset.</returns>
    Task<int> AddAsync(AddDocumentAssetCommand command);

    /// <summary>
    /// Updates the properties of an existing document asset. Updating
    /// the file is optional, but if you do then existing links to the
    /// asset file will redirect to the new asset file.
    /// </summary>
    /// <param name="command">Command parameters.</param>
    Task UpdateAsync(UpdateDocumentAssetCommand command);

    /// <summary>
    /// Updates the properties of an existing document asset. Updating
    /// the file is optional, but if you do then existing links to the
    /// asset file will redirect to the new asset file.
    /// </summary>
    /// <param name="documentAssetId">
    /// Database id of the document to update.
    /// </param>
    /// <param name="commandPatcher">
    /// An action to configure or "patch" a command that's been initialized
    /// with the existing document asset data.
    /// </param>
    Task UpdateAsync(int documentAssetId, Action<UpdateDocumentAssetCommand> commandPatcher);

    /// <summary>
    /// Removes a document asset from the system and
    /// queues any related files or caches to be removed
    /// as a separate process.
    /// </summary>
    /// <param name="documentAssetId">Id of the document asset to delete.</param>
    Task DeleteAsync(int documentAssetId);
}
