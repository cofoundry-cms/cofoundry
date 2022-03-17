namespace Cofoundry.Web.Admin;

public class DocumentsModuleRouteLibrary : AngularModuleRouteLibrary
{
    public const string RoutePrefix = "documents";

    public DocumentsModuleRouteLibrary(AdminSettings adminSettings)
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