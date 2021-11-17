namespace Cofoundry.Web
{
    /// <summary>
    /// Used to get a string that is used to make the auth cookies unique. The 
    /// user area code will be appended to this to make the cookiename, e.g.
    /// "MyAppAuth_COF". By default the cookie namespace is created
    /// using characters from the entry assembly name of your applicaiton, but
    /// you can override this behaviour using the Cofoundry:Auth:CookieNamespace
    /// config setting.
    /// </summary>
    public interface IAuthCookieNamespaceProvider
    {
        string GetNamespace();
    }
}
