namespace Cofoundry.Web;

/// <summary>
/// Used to get a string that is used to make the auth cookies unique. The 
/// user area code will be appended to this to make the cookie name, e.g.
/// "MyAppAuth_COF". By default the cookie namespace is created
/// using characters from the entry assembly name of your application, but
/// you can override this behaviour using the Cofoundry:Users:Cookie:Namespace
/// config setting.
/// </summary>
public interface IAuthCookieNamespaceProvider
{
    /// <summary>
    /// Gets a string that is used to make the auth cookies unique. The 
    /// user area code will be appended to this to make the cookie name, e.g.
    /// "MyAppAuth_COF". By default the cookie namespace is created
    /// using characters from the entry assembly name of your application, but
    /// you can override this behaviour using the Cofoundry:Users:Cookie:Namespace
    /// config setting.
    /// </summary>
    /// <param name="userAreaCode"></param>
    /// <returns></returns>
    string GetNamespace(string userAreaCode);
}
