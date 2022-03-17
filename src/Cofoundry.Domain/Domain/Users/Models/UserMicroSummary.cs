namespace Cofoundry.Domain;

/// <summary>
/// A very minimal representation of a user. Users are partitioned by
/// user area so a user might be a Cofoundry admin user or could belong
/// to a custom user area. Users cannot belong to more than one user area.
/// </summary>
public class UserMicroSummary
{
    /// <summary>
    /// Database id of the user.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// An optional display-friendly name. If <see cref="UsernameOptions.UseAsDisplayName"/> is set to
    /// <see langword="true"/> then this field will be a copy of the <see cref="Username"/> field.
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// Encapsulates a number of status' that a user account can be in.
    /// This status can be used to determine if an account is <see cref="Deleted"/>
    /// or <see cref="Deactivated"/> allowing you to render data or adjust functionality
    /// accordingly.
    /// </summary>
    public UserAccountStatus AccountStatus { get; set; }

    /// <summary>
    /// Each user must be assigned to a user area (but not more than
    /// one).
    /// </summary>
    public UserAreaMicroSummary UserArea { get; set; }
}
