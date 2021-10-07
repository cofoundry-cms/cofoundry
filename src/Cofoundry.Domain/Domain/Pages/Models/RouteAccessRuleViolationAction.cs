namespace Cofoundry.Domain
{
    /// <summary>
    /// Actions to take when a user does not have access to a
    /// resource route (e.g. Page) in the website.
    /// </summary>
    public enum RouteAccessRuleViolationAction
    {
        /// <summary>
        /// An error should be returned e.g. http 403 (forbidden).
        /// </summary>
        Error = 1,

        /// <summary>
        /// The user should be redirected to the login page associated 
        /// with the user area in the access rule.
        /// </summary>
        RedirectToLogin = 2,

        /// <summary>
        /// A "not found" result should be returned e.g. http 404.
        /// </summary>
        NotFound = 3
    }
}
