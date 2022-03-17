namespace Cofoundry.Web.Admin;

public class RolesRouteLibrary : ModuleRouteLibrary
{
    public const string RoutePrefix = "roles";

    public RolesRouteLibrary(AdminSettings adminSettings)
        : base(adminSettings, RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
    {
    }

    public string List()
    {
        return AngularRoute();
    }

    public string New()
    {
        return AngularRoute("new");
    }

    public string Details(int id)
    {
        return AngularRoute(id.ToString());
    }
}