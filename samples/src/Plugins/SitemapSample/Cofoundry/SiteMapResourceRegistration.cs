using Cofoundry.Plugins.SiteMap;

namespace SitemapSample.Cofoundry;

public class SiteMapResourceRegistration : ISiteMapResourceRegistration
{
    public IEnumerable<ISiteMapResource> GetResources()
    {
        yield return new SiteMapResource()
        {
            Url = "/example-manually-registered-page",
            LastModifiedDate = DateTime.UtcNow,
            Priority = 0.7m
        };
    }
}
