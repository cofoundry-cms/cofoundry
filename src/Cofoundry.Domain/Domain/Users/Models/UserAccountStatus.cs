namespace Cofoundry.Domain;

/// <summary>
/// Encapsulates a number of status' that a user account can be in.
/// This status can be used to determine if an account is <see cref="Deleted"/>
/// or <see cref="Deactivated"/> allowing you to render data or adjust functionality
/// accordingly.
/// </summary>
public enum UserAccountStatus
{
    /// <summary>
    /// The default "empty" or unset state. Does not actually map to a valid
    /// user account state.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The user account has been marked as deleted, and cannot be reinstated.
    /// Associated user data ay have been removed in the deletion process but
    /// the record still exists for audit purposes.
    /// </summary>
    Deleted = 1,

    /// <summary>
    /// The user account has been deactivated, removing sigin in access. The user can
    /// be reactivated again at a later date.
    /// </summary>
    Deactivated = 2,

    /// <summary>
    /// The user account is active, allowing the user to sign in.
    /// </summary>
    Active = 3
}
