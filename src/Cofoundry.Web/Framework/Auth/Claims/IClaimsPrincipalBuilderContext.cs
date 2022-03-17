namespace Cofoundry.Web;

/// <summary>
/// Encapsulates key user data that can be used to build an
/// claims principal.
/// </summary>
public interface IClaimsPrincipalBuilderContext
{
    /// <summary>
    /// The user's unique database identifier.
    /// </summary>
    int UserId { get; }

    /// <summary>
    /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area the
    /// user belongs to.
    /// </summary>
    string UserAreaCode { get; }

    /// <summary>
    /// The security stamp is a random string that gets updated when key user identity fields 
    /// are changed, such as a password or username. During user session validation this field is checked to
    /// detect any changes and invalidate any out of date sessions. For example, if I am logged 
    /// into the admin panel on a Latop and a PC, and change my password on the Laptop session, then
    /// I would be logged out of the session on the PC.
    /// </summary>
    string SecurityStamp { get; }
}
