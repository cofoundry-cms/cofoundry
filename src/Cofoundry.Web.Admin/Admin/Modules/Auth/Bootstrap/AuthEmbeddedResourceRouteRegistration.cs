using Cofoundry.Core.ResourceFiles;

namespace Cofoundry.Web.Admin;

public class AuthEmbeddedResourceRouteRegistration : IEmbeddedResourceRouteRegistration
{
    private readonly IAdminRouteLibrary _adminRouteLibrary;
    private readonly AdminSettings _adminSettings;

    public AuthEmbeddedResourceRouteRegistration(
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
            _adminRouteLibrary.Auth.GetStaticResourceFilePath(),
            _adminRouteLibrary.Auth.GetStaticResourceUrlPath()
            );

        yield return path;
    }
}
