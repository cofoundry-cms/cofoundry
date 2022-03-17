namespace Cofoundry.Domain;

/// <summary>
/// A minimal projection of a role with only essential identitifcation data, 
/// which is typically used as part of another entity projection.
/// </summary>
public class RoleMicroSummary
{
    /// <summary>
    /// Database id of the role.
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// The title is used to identify the role and select it in the admin UI.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// A role must be assigned to a user area e.g. <see cref="CofoundryAdminUserArea"/>.
    /// </summary>
    public UserAreaMicroSummary UserArea { get; set; }
}
