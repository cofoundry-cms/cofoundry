using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Plugins.SiteMap;

public class SiteMapController : Controller
{
    private readonly ISiteMapRenderer _siteMapRenderer;

    public SiteMapController(
        IQueryExecutor queryExecutor,
        ISiteMapRenderer siteMapRenderer
        )
    {
        _siteMapRenderer = siteMapRenderer;
    }

    [Route("sitemap.xml")]
    public async Task<ContentResult> SitemapXml()
    {
        var siteMap = await _siteMapRenderer.RenderToUTF8StringAsync();
        return Content(siteMap, "application/xml", Encoding.UTF8);
    }

    [Route("sitemap.xsl")]
    public ContentResult SitemapXsl()
    {
        const string xslResource = "Cofoundry.Plugins.SiteMap.DefaultImplementation.sitemap.xsl";

        var assembly = typeof(SiteMapController).GetTypeInfo().Assembly;

        using var stream = assembly.GetManifestResourceStream(xslResource);

        if (stream == null)
        {
            throw new FileNotFoundException($"Could not find sitemap XLS file at {xslResource}");
        }

        using var reader = new StreamReader(stream);
        var xslContent = reader.ReadToEnd();

        return Content(xslContent, "application/xml", Encoding.UTF8);
    }
}
