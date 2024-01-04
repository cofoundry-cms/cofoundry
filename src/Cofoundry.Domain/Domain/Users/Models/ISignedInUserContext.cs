namespace Cofoundry.Domain;

/// <summary>
/// The authentication status of a signed in user at a specific point 
/// in time, typically representing the current context of a user during
/// the execution of a request. Users can log into multiple user 
/// areas concurrently, so an <see cref="IUserContext"/> is scoped
/// to a specific user area.
/// </summary>
public interface ISignedInUserContext
{
    /// <summary>
    /// Unique identifier for the User.
    /// </summary>
    int UserId { get; }

    /// <summary>
    /// This indicates which User Area they are logged into. Typically the only 
    /// user area will be Cofoundry Admin, but some sites may have additional 
    /// custom user areas e.g. a members area.
    /// </summary>
    IUserAreaDefinition UserArea { get; }

    /// <summary>
    /// Indicates if the user should be required to change thier password when they log on.
    /// </summary>
    bool IsPasswordChangeRequired { get; }

    /// <summary>
    /// Indicates if the account has been marked as verified. Verified accounts are an
    /// optional feature and this property can be ignored if you haven't implemented 
    /// account verification.
    /// </summary>
    bool IsAccountVerified { get; }

    /// <summary>
    /// The unique identifier of the role that this user belongs to.
    /// </summary>
    int RoleId { get; }

    /// <summary>
    /// If the user belongs to a code-first role, then this will be the string identifier
    /// for that role. Otherwise this will be <see langword="null"/>.
    /// </summary>
    string? RoleCode { get; }
}
