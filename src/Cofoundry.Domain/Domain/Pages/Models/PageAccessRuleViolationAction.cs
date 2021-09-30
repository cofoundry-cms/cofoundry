namespace Cofoundry.Domain
{
    /// <summary>
    /// Actions to take when a user does not have access to a
    /// page.
    /// </summary>
    public enum PageAccessRuleViolationAction
    {
        /// <summary>
        /// An error should be returned e.g. http 403 (forbidden).
        /// </summary>
        Error,

        /// <summary>
        /// The user should be redirected to the login page associated 
        /// with the user area in the access rule.
        /// </summary>
        RedirectToLogin,

        /// <summary>
        /// A "not found" result should be returned e.g. http 404.
        /// </summary>
        NotFound
    }
}
