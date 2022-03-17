namespace Cofoundry.Domain;

/// <summary>
/// Contains full document information. This is specifically used in the 
/// admin panel and so contains audit data and tagging information.
/// </summary>
public class DocumentAssetDetails : DocumentAssetSummary
{
    /// <summary>
    /// A longer description of the document in plain text.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The date the file was last updated. Used for cache busting
    /// the asset file in web requests.
    /// </summary>
    public DateTime FileUpdateDate { get; set; }
}
