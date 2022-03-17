namespace Cofoundry.Domain;

/// <summary>
/// Message published when an image is added.
/// </summary>
public class ImageAssetAddedMessage
{
    /// <summary>
    /// Id of the page that was added
    /// </summary>
    public int ImageAssetId { get; set; }
}
