namespace Cofoundry.Web.Admin;

public class DashboardRouteLibrary : AngularModuleRouteLibrary
{
    public const string RoutePrefix = "dashboard";

    public DashboardRouteLibrary(AdminSettings adminSettings)
        : base(adminSettings, RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
    {
    }

    public string Dashboard()
    {
        return AngularRoute();
    }
}