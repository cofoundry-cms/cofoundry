namespace Cofoundry.Web.Admin;

public class PagesModuleRouteLibrary : AngularModuleRouteLibrary
{
    public const string RoutePrefix = "pages";

    private readonly AdminSettings _adminSettings;

    public PagesModuleRouteLibrary(AdminSettings adminSettings)
        : base(adminSettings, RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
    {
        _adminSettings = adminSettings;
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
        return AngularRoute(id.ToString(CultureInfo.InvariantCulture));
    }

    public string PageDirectoryList()
    {
        return CreatePageDirectoryRoute();
    }

    private string CreatePageDirectoryRoute(string? path = null)
    {
        return _adminSettings.DirectoryName + "/" + UrlPrefix + "#/directories/" + path;
    }
}
