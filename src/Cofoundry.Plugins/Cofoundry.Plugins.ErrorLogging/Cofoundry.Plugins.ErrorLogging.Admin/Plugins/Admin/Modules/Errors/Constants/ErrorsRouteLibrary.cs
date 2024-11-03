using System.Globalization;
using Cofoundry.Domain;
using Cofoundry.Web.Admin;

namespace Cofoundry.Plugins.ErrorLogging.Admin;

public class ErrorsRouteLibrary : ModuleRouteLibrary
{
    public const string RoutePrefix = "errors";

    public ErrorsRouteLibrary(AdminSettings adminSetting)
        : base(adminSetting, RoutePrefix, RouteConstants.PluginModuleResourcePathPrefix)
    {
    }

    public string List()
    {
        return AngularRoute();
    }

    public string Details(int id)
    {
        return AngularRoute(id.ToString(CultureInfo.InvariantCulture));
    }
}
