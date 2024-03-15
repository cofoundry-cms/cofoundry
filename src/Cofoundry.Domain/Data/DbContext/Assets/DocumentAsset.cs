namespace Cofoundry.Domain.Data;

/// <summary>
/// Represents a non-image file that has been uploaded to the 
/// CMS. The name could be misleading here as any file type except
/// images are supported, but at least it is less ambigous than the 
/// term 'file'.
/// </summary>
public class DocumentAsset : IUpdateAuditable
{
    /// <summary>
    /// Database id of the document asset.
    /// </summary>
    public int DocumentAssetId { get; set; }

    /// <summary>
    /// The filename is taken from the title property
    /// and cleaned to remove invalid characters.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

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
    /// File name used internally for storing the file on disk (without 
    /// extension). This is typically in the format {ImageAssetId}-{FileStamp}.
    /// </summary>
    /// <remarks>
    /// For files created before file stamps were used this may
    /// contain only the image asset id.
    /// </remarks>
    public string FileNameOnDisk { get; set; } = string.Empty;

    /// <summary>
    /// A short descriptive title of the document (130 characters).
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// A longer description of the document in plain text.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The length of the document file, in bytes.
    /// </summary>
    public long FileSizeInBytes { get; set; }

    /// <summary>
    /// The date the file was last updated. Used for cache busting
    /// the asset file in web requests.
    /// </summary>
    public DateTime FileUpdateDate { get; set; }

    /// <summary>
    /// The MIME type used to describe the file in the content-type header of
    /// an http request. This can be <see langword="null"/> if the content
    /// type could not be determined.
    /// </summary>
    public string? ContentType { get; set; }

    /// <inheritdoc/>
    public DateTime CreateDate { get; set; }

    /// <inheritdoc/>
    public int CreatorId { get; set; }

    private User? _creator;
    /// <inheritdoc/>
    public User Creator
    {
        get => _creator ?? throw NavigationPropertyNotInitializedException.Create<DocumentAsset>(nameof(Creator));
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
        get => _updater ?? throw NavigationPropertyNotInitializedException.Create<DocumentAsset>(nameof(Creator));
        set => _updater = value;
    }

    [Obsolete("The document asset grouping system will be revised in an upcomming release.")]
    public ICollection<DocumentAssetGroupItem> DocumentAssetGroupItems { get; set; } = new List<DocumentAssetGroupItem>();

    public ICollection<DocumentAssetTag> DocumentAssetTags { get; set; } = new List<DocumentAssetTag>();
}
