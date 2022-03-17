namespace Cofoundry.Web.Admin;

public class AccountRouteLibrary : AngularModuleRouteLibrary
{
    public const string RoutePrefix = "account";

    public AccountRouteLibrary(AdminSettings adminSettings)
        : base(adminSettings, RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
    {
    }

    public string Details()
    {
        return AngularRoute();
    }
}