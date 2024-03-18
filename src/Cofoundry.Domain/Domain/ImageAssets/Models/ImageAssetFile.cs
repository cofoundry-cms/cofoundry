﻿namespace Cofoundry.Domain;

/// <summary>
/// Represents the file associated with a document asset, including
/// stream access to the file itself.
/// </summary>
public class ImageAssetFile : IImageAssetRenderable
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
    /// The date the file was last updated. Used for cache busting
    /// the asset file in web requests.
    /// </summary>
    public DateTime FileUpdateDate { get; set; }

    /// <summary>
    /// The default Anchor Location when using dynamic cropping
    /// </summary>
    public ImageAnchorLocation? DefaultAnchorLocation { get; set; }

    /// <summary>
    /// A stream containing the contents of the file. This needs
    /// to be disposed of when you've finished with it.
    /// </summary>
    public Stream ContentStream { get; set; } = Stream.Null;
}
