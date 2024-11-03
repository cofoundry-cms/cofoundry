namespace Cofoundry.Plugins.SiteMap;

/// <summary>
/// Resgitration for resources that should appear in an xml site map file
/// </summary>
public interface ISiteMapResourceRegistration
{
    IEnumerable<ISiteMapResource> GetResources();
}
