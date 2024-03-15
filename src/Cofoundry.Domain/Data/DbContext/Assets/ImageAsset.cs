namespace Cofoundry.Domain.Data;

/// <summary>
/// Represents an image file that has been uploaded to the CMS.
/// </summary>
public class ImageAsset : IUpdateAuditable
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
    /// File name used internally for storing the file on disk (without 
    /// extension). This is typically in the format {ImageAssetId}-{FileStamp}.
    /// </summary>
    /// <remarks>
    /// For files created before file stamps were used this may
    /// contain only the image asset id.
    /// </remarks>
    public string FileNameOnDisk { get; set; } = string.Empty;

    /// <summary>
    /// Original file extension without the leading dot.
    /// </summary>
    public string FileExtension { get; set; } = string.Empty;

    /// <summary>
    /// A random string token that can be used to verify a file request
    /// and mitigate enumeration attacks.
    /// </summary>
    public string VerificationToken { get; set; } = string.Empty;

    /// <summary>
    /// The width of the image file in pixels.
    /// </summary>
    public int WidthInPixels { get; set; }

    /// <summary>
    /// The height of the image file in pixels.
    /// </summary>
    public int HeightInPixels { get; set; }

    /// <summary>
    /// The focal point to use when using dynamic cropping
    /// by default. 
    /// </summary>
    public ImageAnchorLocation? DefaultAnchorLocation { get; set; }

    /// <summary>
    /// The title or alt text for an image. Recommended to be up 
    /// 125 characters to accomodate screen readers.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Size of the image file on disk.
    /// </summary>
    public long FileSizeInBytes { get; set; }

    /// <summary>
    /// The date the file was last updated. Used for cache busting
    /// the asset file in web requests.
    /// </summary>
    public DateTime FileUpdateDate { get; set; }

    /// <inheritdoc/>
    public DateTime CreateDate { get; set; }

    /// <inheritdoc/>
    public int CreatorId { get; set; }

    private User? _creator;
    /// <inheritdoc/>
    public User Creator
    {
        get => _creator ?? throw NavigationPropertyNotInitializedException.Create<ImageAsset>(nameof(Creator));
        set => _creator = value;
    }

    /// <inheritdoc/>
    public DateTime UpdateDate { get; set; }

    /// <inheritdoc/>
    public int UpdaterId { get; set; }

    private User? _updater;
    /// <inheritdoc/>
    public User Updater
    {
        get => _updater ?? throw NavigationPropertyNotInitializedException.Create<ImageAsset>(nameof(Creator));
        set => _updater = value;
    }

    [Obsolete("The image asset grouping system will be revised in an upcomming release.")]
    public ICollection<ImageAssetGroupItem> ImageAssetGroupItems { get; set; } = new List<ImageAssetGroupItem>();

    /// <summary>
    /// Tags can be used to categorize an entity.
    /// </summary>
    public ICollection<ImageAssetTag> ImageAssetTags { get; set; } = new List<ImageAssetTag>();
}
