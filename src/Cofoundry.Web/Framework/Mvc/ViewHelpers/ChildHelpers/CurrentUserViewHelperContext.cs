namespace Cofoundry.Web.Internal;

/// <summary>
/// Collects together information about a user that can be
/// used in a view.
/// </summary>
public class CurrentUserViewHelperContext : ICurrentUserViewHelperContext
{
    /// <summary>
    /// Please use <see cref="IsSignedIn"/> instead.
    /// </summary>
    [Obsolete("This has been renamed to 'IsSignedIn'")]
    public bool IsLoggedIn { get { return IsSignedIn; } }

    /// <summary>
    /// Please use <see cref="Data"/> instead.
    /// </summary>
    [Obsolete("This has been renamed to 'Data'.")]
    public UserSummary User { get { return Data; } }

    /// <summary>
    /// <see langword="true"/> whether the user is signed in; otherwise
    /// <see langword="false"/>.
    /// </summary>
    public bool IsSignedIn { get; set; }

    /// <summary>
    /// Information about the currently logged in user. If the user is
    /// not signed in the this will be <see langword="null"/>.
    /// </summary>
    public UserSummary Data { get; set; }

    /// <summary>
    /// Information about the role that the currently signed in user 
    /// belongs to.
    /// </summary>
    public RoleDetails Role { get; set; }
}
