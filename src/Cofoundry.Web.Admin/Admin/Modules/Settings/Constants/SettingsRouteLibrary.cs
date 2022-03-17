namespace Cofoundry.Web.Admin;

public class SettingsRouteLibrary : ModuleRouteLibrary
{
    public const string RoutePrefix = "settings";

    public SettingsRouteLibrary(AdminSettings adminSettings)
        : base(adminSettings, RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
    {
    }

    public string Details()
    {
        return AngularRoute();
    }
}