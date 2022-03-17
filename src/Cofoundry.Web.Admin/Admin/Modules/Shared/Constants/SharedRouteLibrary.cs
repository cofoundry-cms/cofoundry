namespace Cofoundry.Web.Admin;

public class SharedRouteLibrary : AngularModuleRouteLibrary
{
    public const string RoutePrefix = "shared";

    public SharedRouteLibrary(AdminSettings adminSettings)
        : base(adminSettings, RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
    {
    }
}
