namespace Cofoundry.Web.Admin;

public class DirectoriesRouteLibrary : AngularModuleRouteLibrary
{
    public const string RoutePrefix = "directories";

    public DirectoriesRouteLibrary(AdminSettings adminSettings)
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