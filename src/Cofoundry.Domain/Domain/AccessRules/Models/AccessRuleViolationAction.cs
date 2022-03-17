namespace Cofoundry.Domain;

/// <summary>
/// Actions to take when a user does not have access to a
/// resource route (e.g. Page) in the website.
/// </summary>
public enum AccessRuleViolationAction
{
    /// <summary>
    /// An error should be returned e.g. http 403 (forbidden).
    /// </summary>
    Error = 0,

    /// <summary>
    /// A "not found" result should be returned e.g. http 404.
    /// </summary>
    NotFound = 1
}
