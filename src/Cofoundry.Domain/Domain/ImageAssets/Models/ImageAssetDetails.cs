namespace Cofoundry.Domain;

/// <summary>
/// Contains full image asset information. This is specifically used in the 
/// admin panel and so contains audit data and tagging information.
/// </summary>
public class ImageAssetDetails : ImageAssetSummary
{
    /// <summary>
    /// The date the file was last updated. Used for cache busting
    /// the asset file in web requests.
    /// </summary>
    public DateTime FileUpdateDate { get; set; }
}
