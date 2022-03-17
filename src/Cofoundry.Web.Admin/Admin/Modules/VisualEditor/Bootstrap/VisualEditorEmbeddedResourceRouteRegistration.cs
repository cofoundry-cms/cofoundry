using Cofoundry.Core.ResourceFiles;

namespace Cofoundry.Web.Admin;

public class VisualEditorEmbeddedResourceRouteRegistration : IEmbeddedResourceRouteRegistration
{
    private readonly IAdminRouteLibrary _adminRouteLibrary;
    private readonly AdminSettings _adminSettings;

    public VisualEditorEmbeddedResourceRouteRegistration(
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

        var assembly = GetType().Assembly;

        yield return new EmbeddedResourcePath(
            assembly,
            _adminRouteLibrary.VisualEditor.GetStaticResourceFilePath(),
            _adminRouteLibrary.VisualEditor.GetStaticResourceUrlPath()
            );
    }
}
