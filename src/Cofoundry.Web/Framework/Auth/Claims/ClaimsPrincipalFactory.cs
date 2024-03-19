﻿using System.Security.Claims;

namespace Cofoundry.Web.Internal;

/// <summary>
/// Default implementation of <see cref="IClaimsPrincipalFactory"/>.
/// </summary>
public class ClaimsPrincipalFactory : IClaimsPrincipalFactory
{
    /// <inheritdoc/>
    public Task<ClaimsPrincipal> CreateAsync(IClaimsPrincipalBuilderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

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
