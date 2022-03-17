namespace Cofoundry.Web.Admin;

public class ImagesModuleRouteLibrary : AngularModuleRouteLibrary
{
    public const string RoutePrefix = "images";

    public ImagesModuleRouteLibrary(AdminSettings adminSettings)
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