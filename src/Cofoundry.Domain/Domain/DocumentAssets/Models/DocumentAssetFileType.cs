namespace Cofoundry.Domain;

/// <summary>
/// Represents a file type currently stored in the document assets
/// database table.
/// </summary>
public class DocumentAssetFileType
{
    /// <summary>
    /// The file system file extension without the
    /// leading dot.
    /// </summary>
    public string FileExtension { get; set; }
}
