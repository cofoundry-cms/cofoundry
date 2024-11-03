using Cofoundry.Plugins.SiteMap;

namespace SitemapExample.Cofoundry;

public class SiteMapResourceRegistration : ISiteMapResourceRegistration
{
    public IEnumerable<ISiteMapResource> GetResources()
    {
        yield return new SiteMapResource()
        {
            Url = "/test",
            LastModifiedDate = DateTime.UtcNow,
            Priority = 0.7m
        };
    }
}
