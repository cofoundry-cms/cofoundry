namespace Cofoundry.Domain;

/// <summary>
/// This message is published when an image is updated.
/// </summary>
public class ImageAssetUpdatedMessage
{
    /// <summary>
    /// Id of the image that has been updated
    /// </summary>
    public int ImageAssetId { get; set; }

    /// <summary>
    /// Indicates if a new file was uploaded to replace the existing one.
    /// </summary>
    public bool HasFileChanged { get; set; }
}
