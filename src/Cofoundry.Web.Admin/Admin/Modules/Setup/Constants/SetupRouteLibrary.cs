namespace Cofoundry.Web.Admin;

public class SetupRouteLibrary : AngularModuleRouteLibrary
{
    public const string RoutePrefix = "setup";

    public const string SetupLayoutPath = "~/Admin/Modules/Setup/MVC/Views/_SetupLayout.cshtml";

    public SetupRouteLibrary(AdminSettings adminSettings)
        : base(adminSettings, RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
    {
    }

    public string Setup()
    {
        return MvcRoute();
    }
}