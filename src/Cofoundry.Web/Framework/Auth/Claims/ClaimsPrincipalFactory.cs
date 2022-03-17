using System.Security.Claims;

namespace Cofoundry.Web.Internal;

/// <inheritdoc/>
public class ClaimsPrincipalFactory : IClaimsPrincipalFactory
{
    public Task<ClaimsPrincipal> CreateAsync(IClaimsPrincipalBuilderContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        var scheme = AuthenticationSchemeNames.UserArea(context.UserAreaCode);
        var claims = new[]
        {
            new Claim(CofoundryClaimTypes.UserId, Convert.ToString(context.UserId)),
            new Claim(CofoundryClaimTypes.SecurityStamp, context.SecurityStamp),
            new Claim(CofoundryClaimTypes.UserAreaCode, context.UserAreaCode),
        };

        var claimsIdentity = new ClaimsIdentity(claims, scheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        return Task.FromResult(claimsPrincipal);
    }
}
