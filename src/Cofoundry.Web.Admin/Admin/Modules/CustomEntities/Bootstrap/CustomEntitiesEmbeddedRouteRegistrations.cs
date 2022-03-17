using Cofoundry.Core.ResourceFiles;

namespace Cofoundry.Web.Admin;

public class CustomEntitiesResourceRouteRegistration : IEmbeddedResourceRouteRegistration
{
    private readonly IAdminRouteLibrary _adminRouteLibrary;
    private readonly AdminSettings _adminSettings;

    public CustomEntitiesResourceRouteRegistration(
        IAdminRouteLibrary adminRouteLibrary,
        AdminSettings adminSettings
        )
    {
        _adminRouteLibrary = adminRouteLibrary;
        _adminSettings = adminSettings;
    }

    public IEnumerable<EmbeddedResourcePath> GetEmbeddedResourcePaths()
    {
        if (_adminSettings.Disabled) yield break;

        var path = new EmbeddedResourcePath(
            GetType().Assembly,
            _adminRouteLibrary.CustomEntities.GetStaticResourceFilePath(),
            _adminRouteLibrary.CustomEntities.GetStaticResourceUrlPath()
            );

        yield return path;
    }
}
