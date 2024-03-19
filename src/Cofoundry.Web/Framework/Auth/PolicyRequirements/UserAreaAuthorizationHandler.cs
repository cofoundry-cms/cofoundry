﻿using Microsoft.AspNetCore.Authorization;

namespace Cofoundry.Web.Auth.Internal;

/// <summary>
/// Handles the authorization of the <see cref="UserAreaAuthorizationRequirement"/> in
/// an authorization policy.
/// </summary>
public class UserAreaAuthorizationHandler : AuthorizationHandler<UserAreaAuthorizationRequirement>
{
    private readonly IUserContextService _userContextService;

    public UserAreaAuthorizationHandler(
        IUserContextService userContextService
        )
    {
        _userContextService = userContextService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserAreaAuthorizationRequirement requirement)
    {
        var user = await _userContextService.GetCurrentContextAsync();
        var signedInUser = user.ToSignedInContext();

        if (signedInUser != null && signedInUser.UserArea.UserAreaCode == requirement.UserAreaCode)
        {
            context.Succeed(requirement);
        }
    }
}
