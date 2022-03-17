namespace Cofoundry.Web.Admin;

public class PageTemplatesRouteLibrary : AngularModuleRouteLibrary
{
    public const string RoutePrefix = "page-templates";

    public PageTemplatesRouteLibrary(AdminSettings adminSettings)
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