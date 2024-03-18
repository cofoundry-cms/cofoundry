﻿namespace Cofoundry.Domain;

/// <summary>
/// Represents the file associated with a document asset, including
/// stream access to the file itself.
/// </summary>
public class DocumentAssetFile : IDocumentAssetRenderable
{
    /// <summary>
    /// Database id of the document asset.
    /// </summary>
    public int DocumentAssetId { get; set; }

    /// <summary>
    /// The MIME type used to describe the file in the content-type header of
    /// an http request. This can be <see langword="null"/> if the content
    /// type could not be determined.
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// The filename is taken from the title property
    /// and cleaned to remove invalid characters.
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
    /// A stream containing the contents of the file. This needs
    /// to be disposed of when you've finished with it.
    /// </summary>
    public Stream ContentStream { get; set; } = Stream.Null;

    /// <summary>
    /// Gets the full filename including the file extension. The 
    /// filename is cleaned to remove invalid characters.
    /// </summary>
    public string GetFileNameWithExtension()
    {
        if (FileName == null)
        {
            return string.Empty;
        }

        var fileName = FilePathHelper.CleanFileName(FileName, DocumentAssetId.ToString());
        var fileNameWithExtension = Path.ChangeExtension(fileName, FileExtension);

        return fileNameWithExtension;
    }
}
