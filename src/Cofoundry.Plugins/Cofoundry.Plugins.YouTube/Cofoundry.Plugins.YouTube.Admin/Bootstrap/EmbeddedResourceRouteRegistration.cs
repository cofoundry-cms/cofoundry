using Cofoundry.Core;
using Cofoundry.Core.ResourceFiles;
using Cofoundry.Domain;
using Cofoundry.Web.Admin;

namespace Cofoundry.Plugins.YouTube.Admin.Bootstrap;

public class EmbeddedResourceRouteRegistration : IEmbeddedResourceRouteRegistration
{
    private readonly AdminSettings _adminSettings;

    public EmbeddedResourceRouteRegistration(AdminSettings adminSettings)
    {
        _adminSettings = adminSettings;
    }

    public IEnumerable<EmbeddedResourcePath> GetEmbeddedResourcePaths()
    {
        if (_adminSettings.Disabled)
        {
            yield break;
        }

        var path = RouteConstants.PluginModuleResourcePathPrefix + "Shared/Content/";
        var rewritePath = RelativePathHelper.Combine(_adminSettings.DirectoryName, path);

        yield return new EmbeddedResourcePath(GetType().Assembly, path, rewritePath);
    }
}
