namespace Cofoundry.Domain;

/// <summary>
/// Gets a range of image assets by their ids projected as a set of
/// ImageAssetRenderDetails models. An ImageAssetRenderDetails 
/// contains all the basic information required to render out an image asset
/// to a page, including all the data needed to construct an asset file 
/// url.
/// </summary>
public class GetImageAssetRenderDetailsByIdRangeQuery : IQuery<IReadOnlyDictionary<int, ImageAssetRenderDetails>>
{
    public GetImageAssetRenderDetailsByIdRangeQuery()
    {
        ImageAssetIds = new List<int>();
    }

    /// <summary>
    /// Initializes the query with parameters.
    /// </summary>
    /// <param name="imageAssetIds">Collection of database ids of the image assets to get.</param>
    public GetImageAssetRenderDetailsByIdRangeQuery(
        IEnumerable<int> imageAssetIds
        )
        : this(imageAssetIds?.ToArray() ?? [])
    {
    }

    /// <summary>
    /// Initializes the query with parameters.
    /// </summary>
    /// <param name="imageAssetIds">Collection of database ids of the image assets to get.</param>
    public GetImageAssetRenderDetailsByIdRangeQuery(
        IReadOnlyCollection<int> imageAssetIds
        )
    {
        ArgumentNullException.ThrowIfNull(imageAssetIds);

        ImageAssetIds = imageAssetIds;
    }

    /// <summary>
    /// Collection of database ids of the image assets to get.
    /// </summary>
    [Required]
    public IReadOnlyCollection<int> ImageAssetIds { get; set; }
}
