namespace Cofoundry.Domain;

/// <summary>
/// This projection that is typically used as part of an aggregate rather
/// than by itself and contains page region and block data for a custom
/// entity details page.
/// </summary>
public class CustomEntityPage
{
    /// <summary>
    /// Information about the page this instance is associated with.
    /// </summary>
    public PageRoute PageRoute { get; set; } = PageRoute.Uninitialized;

    /// <summary>
    /// The full path of the page including directories and the locale. 
    /// </summary>
    public string FullUrlPath { get; set; } = string.Empty;

    /// <summary>
    /// All region and block data for this custom entity page.
    /// </summary>
    public IReadOnlyCollection<CustomEntityPageRegionDetails> Regions { get; set; } = Array.Empty<CustomEntityPageRegionDetails>();
}
