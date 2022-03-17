using Cofoundry.Core.Web;

namespace Cofoundry.Web.Admin;

public class AuthRouteLibrary : ModuleRouteLibrary
{
    public const string RoutePrefix = "auth";

    public readonly string LoginLayoutPath = ViewPathFormatter.View("Auth", "_LoginLayout");

    public AuthRouteLibrary(AdminSettings adminSettings)
        : base(adminSettings, RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
    {
    }

    public string Login(string returnUrl = null)
    {
        var qs = QueryStringBuilder.Create("ReturnUrl", returnUrl);

        return MvcRoute("login", qs);
    }

    public string ChangePassword(string returnUrl = null)
    {
        var qs = QueryStringBuilder.Create("ReturnUrl", returnUrl);

        return MvcRoute("change-password", qs);
    }

    public string ForgotPassword()
    {
        return MvcRoute("forgot-password");
    }

    public string LogOut()
    {
        return MvcRoute("logout");
    }

    /// <summary>
    /// The base url for account recovery requests i.e. without the
    /// required query parameters.
    /// </summary>
    public string ResetPasswordBase()
    {
        return MvcRoute("reset-password");
    }
}
