using System.Globalization;
using System.Text;
using System.Xml.Linq;
using Cofoundry.Core.Validation;
using Cofoundry.Core.Web;

namespace Cofoundry.Plugins.SiteMap;

/// <summary>
/// Default implementation of <see cref="ISiteMapBuilder"/>.
/// </summary>
public class SiteMapBuilder : ISiteMapBuilder
{
    private readonly ISiteUrlResolver _uriResolver;
    private readonly IModelValidationService _modelValidationService;

    public SiteMapBuilder(
        ISiteUrlResolver uriResolver,
        IModelValidationService modelValidationService
        )
    {
        _uriResolver = uriResolver;
        _modelValidationService = modelValidationService;
    }

    /// <summary>
    /// The resources to include in the site map. These will be autiomatically
    /// ordered by priority when rendering.
    /// </summary>
    public List<ISiteMapResource> Resources { get; set; } = [];

    /// <summary>
    /// Creates the SiteMap xml document.
    /// </summary>
    public XDocument ToXml()
    {
        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
        var doc = new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                new XProcessingInstruction("xml-stylesheet", "type=\"text/xsl\" href=\"sitemap.xsl\""));

        var urlset = new XElement(ns + "urlset",
                    new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                    new XAttribute(xsi + "schemaLocation", "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd"));

        doc.Add(urlset);

        foreach (var resource in Resources
            .OrderByDescending(p => p.Priority)
            .ThenBy(p => p.Url))
        {
            _modelValidationService.Validate(resource);
            var el = new XElement(ns + "url", new XElement(ns + "loc", _uriResolver.MakeAbsolute(resource.Url)));

            if (resource.LastModifiedDate.HasValue)
            {
                el.Add(new XElement(ns + "lastmod", resource.LastModifiedDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
            }

            if (resource.Priority.HasValue)
            {
                el.Add(new XElement(ns + "priority", resource.Priority.Value.ToString("0.0", CultureInfo.InvariantCulture)));
            }

            urlset.Add(el);
        }

        return doc;
    }

    /// <summary>
    /// Renders the builder to a string with UTF8 encoding.
    /// </summary>
    public override string ToString()
    {
        var builder = new StringBuilder();
        var doc = ToXml();
        using (var writer = new Utf8StringWriter(builder, CultureInfo.InvariantCulture))
        {
            doc.Save(writer, SaveOptions.None);
        }
        return builder.ToString();
    }
}
