namespace Cofoundry.Domain;

/// <summary>
/// Contains the key information for displaying the image asset in a 
/// list, specifically used in the admin panel and therefore contains audit 
/// data and tagging information.
/// </summary>
public class ImageAssetSummary : IImageAssetRenderable, IUpdateAudited
{
    /// <summary>
    /// Database if of the image asset.
    /// </summary>
    public int ImageAssetId { get; set; }

    /// <summary>
    /// Original filename without an extension.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Original file extension without the leading dot.
    /// </summary>
    public string FileExtension { get; set; } = string.Empty;

    /// <summary>
    /// The width of the image file in pixels.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// The height of the image file in pixels.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// The title or alt text for an image. Recommended to be up 125 characters 
    /// to accomodate screen readers.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// An identifier linked to the physical file that can be used for
    /// cache busting. By default this is a timestamp.
    /// </summary>
    public string FileStamp { get; set; } = string.Empty;

    /// <summary>
    /// A random string token that can be used to verify a file request
    /// and mitigate enumeration attacks.
    /// </summary>
    public string VerificationToken { get; set; } = string.Empty;

    /// <summary>
    /// File name used internally for storing the file on disk (without 
    /// extension). This is typically in the format {ImageAssetId}-{FileStamp}.
    /// </summary>
    /// <remarks>
    /// For files created before file stamps were used this may
    /// contain only the image asset id.
    /// </remarks>
    public string FileNameOnDisk { get; set; } = string.Empty;

    /// <summary>
    /// Size of the image file on disk.
    /// </summary>
    public long FileSizeInBytes { get; set; }

    /// <summary>
    /// The default Anchor Location when using dynamic cropping
    /// </summary>
    public ImageAnchorLocation? DefaultAnchorLocation { get; set; }

    /// <summary>
    /// The base url to display the image.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Tags can be used to categorize an entity.
    /// </summary>
    public IReadOnlyCollection<string> Tags { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Data detailing who created and updated the image asset and when.
    /// </summary>
    public UpdateAuditData AuditData { get; set; } = UpdateAuditData.Uninitialized;
}
