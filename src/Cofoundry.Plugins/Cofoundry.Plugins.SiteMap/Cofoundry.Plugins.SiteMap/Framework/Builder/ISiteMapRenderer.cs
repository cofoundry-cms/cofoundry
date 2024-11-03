namespace Cofoundry.Plugins.SiteMap;

/// <summary>
/// A helper class that renders all registered site map resources
/// to an xml sitemap string with UTF8 encoding
/// </summary>
public interface ISiteMapRenderer
{
    /// <summary>
    /// Renders all registered site map resources
    /// to an xml sitemap string with UTF8 encoding
    /// </summary>
    Task<string> RenderToUTF8StringAsync();
}
