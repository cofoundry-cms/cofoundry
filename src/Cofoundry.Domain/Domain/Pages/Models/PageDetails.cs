namespace Cofoundry.Domain;

/// <summary>
/// Primarily used in the admin area, the PageDetails object includes
/// audit data and other additional information that should normally be 
/// hidden from a customer facing app.
/// </summary>
public class PageDetails : ICreateAudited
{
    /// <summary>
    /// Database id of the page record.
    /// </summary>
    public int PageId { get; set; }

    /// <summary>
    /// The routing data for the page.
    /// </summary>
    public PageRoute PageRoute { get; set; } = PageRoute.Uninitialized;

    /// <summary>
    /// These tags are used in the admin panel for searching and categorizing.
    /// </summary>
    public IReadOnlyCollection<string> Tags { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Data for the latest version of the page, which is not
    /// neccessarily published.
    /// </summary>
    public PageVersionDetails LatestVersion { get; set; } = PageVersionDetails.Uninitialized;

    /// <summary>
    /// Simple audit data for page creation.
    /// </summary>
    public CreateAuditData AuditData { get; set; } = CreateAuditData.Uninitialized;
}
