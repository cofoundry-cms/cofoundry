using System.Xml.Linq;

namespace Cofoundry.Plugins.SiteMap;

/// <summary>
/// Builds a site map xml file from a set of resources. Call <see cref="ToString"/>
/// to render the xml file to a UTF8 string.
/// </summary>
public interface ISiteMapBuilder
{
    /// <summary>
    /// The resources to include in the site map. These will be automatically
    /// ordered by priority when rendering.
    /// </summary>
    List<ISiteMapResource> Resources { get; set; }

    /// <summary>
    /// Creates the SiteMap xml document.
    /// </summary>
    XDocument ToXml();

    /// <summary>
    /// Renders the builder as a string with UTF8 encoding.
    /// </summary>
    string ToString();
}
