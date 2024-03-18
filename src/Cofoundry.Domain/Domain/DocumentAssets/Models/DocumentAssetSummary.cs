﻿namespace Cofoundry.Domain;

/// <summary>
/// Contains the key information for displaying the document in a 
/// list, specifically used in the admin panel and so contains audit 
/// data and tagging information.
/// </summary>
public class DocumentAssetSummary : IUpdateAudited, IDocumentAssetRenderable
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
    /// A short descriptive title of the document (130 characters).
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The length of the document file, in bytes.
    /// </summary>
    public long FileSizeInBytes { get; set; }

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
    /// The url to display the document asset.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// The url for a document asset that is set to download using
    /// the "attachment" content disposition.
    /// </summary>
    public string DownloadUrl { get; set; } = string.Empty;

    /// <summary>
    /// Tags can be used to categorize an entity.
    /// </summary>
    public IReadOnlyCollection<string> Tags { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Data detailing who created and updated the document asset and when.
    /// </summary>
    public UpdateAuditData AuditData { get; set; } = UpdateAuditData.Uninitialized;
}
