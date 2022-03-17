using System.Security.Claims;

namespace Cofoundry.Web.Internal;

/// <summary>
/// Factory for creating a <see cref="ClaimsPrincipal"/> to represent
/// a Cofoundry user.
/// </summary>
public interface IClaimsPrincipalFactory
{
    /// <summary>
    /// Creates a new claims <see cref="ClaimsPrincipal"/> to represent
    /// the Cofoundry user represented by the specified <paramref name="context"/>.
    /// </summary>
    /// <param name="context">
    /// The <see cref="IClaimsPrincipalBuilderContext"/> contains the key
    /// user data fields required to build the <see cref="ClaimsPrincipal"/>.
    /// </param>
    /// <returns>
    /// The factory should always return a new <see cref="ClaimsPrincipal"/>.
    /// </returns>
    Task<ClaimsPrincipal> CreateAsync(IClaimsPrincipalBuilderContext context);
}
