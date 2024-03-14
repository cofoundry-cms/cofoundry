namespace Cofoundry.Web;

/// <summary>
/// Collects together information about a user that can be
/// used in a view.
/// </summary>
public interface ICurrentUserViewHelperContext
{
    /// <summary>
    /// <see langword="true"/> whether the user is signed in; otherwise
    /// <see langword="false"/>.
    /// </summary>
    bool IsSignedIn { get; }

    /// <summary>
    /// Information about the currently logged in user. If the user is
    /// not signed in the this will be <see langword="null"/>.
    /// </summary>
    UserSummary? Data { get; }

    /// <summary>
    /// Information about the role that the currently signed in user 
    /// belongs to.
    /// </summary>
    RoleDetails Role { get; }
}
