namespace Cofoundry.Domain;

/// <summary>
/// Gets a range of document assets by their ids projected as 
/// DocumentAssetRenderDetails models. A DocumentAssetRenderDetails 
/// contains all the basic information required to render out a document
/// to page, including all the data needed to construct a document file 
/// url.
/// </summary>
public class GetDocumentAssetRenderDetailsByIdRangeQuery : IQuery<IReadOnlyDictionary<int, DocumentAssetRenderDetails>>
{
    public GetDocumentAssetRenderDetailsByIdRangeQuery()
    {
        DocumentAssetIds = new List<int>();
    }

    /// <summary>
    /// Initializes the query with parameters.
    /// </summary>
    /// <param name="documentAssetIds">Collection of database ids of the document assets to get.</param>
    public GetDocumentAssetRenderDetailsByIdRangeQuery(
        IEnumerable<int> documentAssetIds
        )
        : this(documentAssetIds?.ToArray() ?? [])
    {
    }

    /// <summary>
    /// Initializes the query with parameters.
    /// </summary>
    /// <param name="documentAssetIds">Collection of database ids of the document assets to get.</param>
    public GetDocumentAssetRenderDetailsByIdRangeQuery(
        IReadOnlyCollection<int> documentAssetIds
        )
    {
        ArgumentNullException.ThrowIfNull(documentAssetIds);

        DocumentAssetIds = documentAssetIds;
    }

    /// <summary>
    /// Collection of database ids of the document assets to get.
    /// </summary>
    [Required]
    public IReadOnlyCollection<int> DocumentAssetIds { get; set; }
}
