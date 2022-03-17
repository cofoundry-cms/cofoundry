using Microsoft.AspNetCore.Authentication.Cookies;

namespace Cofoundry.Web.Internal;

/// <summary>
/// <para>
/// Used to validate the claims principal for a Cofoundry user during the 
/// <see cref="CookieAuthenticationOptions.Events.OnValidatePrincipal"/> event.
/// </para>
/// <para>
/// The default implementation is the Cofoundry equivalent of <see cref="Microsoft.AspNetCore.Identity.ISecurityStampValidator"/>
/// and is primarily concerned with periodically checking that the security stamp
/// in the claims principal matches the one stored against the user in the database. 
/// The security stamp is changed when key user properties are changed e.g. a username 
/// or password; if the security stamp does not match then the claims principal (i.e. their cookie)
/// is rejected, thus ensuring that a user is logged out of all other devices when their 
/// credentials are changed. Note that when a user changes their credentials, their
/// claims principal is refreshed on the session that invoked the change, so they won't
/// be kicked out of their active session.
/// </para>
/// </summary>
public interface IClaimsPrincipalValidator
{
    /// <summary>
    /// Validates the security stamp claim from the claim principal in the 
    /// <paramref name="validationContext"/>. If the validation is successful 
    /// then the principal is refreshed, otherwise the principal is rejected 
    /// and the user session is abandoned.
    /// </summary>
    /// <param name="validationContext">A context object representing the parameters of the cookie validation event.</param>
    Task ValidateAsync(CookieValidatePrincipalContext validationContext);
}
