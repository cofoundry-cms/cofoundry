using Microsoft.AspNetCore.Authorization;

namespace Cofoundry.Web.Auth.Internal;

/// <summary>
/// Handles the authorization of the <see cref="RoleAuthorizationRequirement"/> in
/// an authorization policy.
/// </summary>
public class RoleAuthorizationHandler : AuthorizationHandler<RoleAuthorizationRequirement>
{
    private readonly IUserContextService _userContextService;

    public RoleAuthorizationHandler(
        IUserContextService userContextService
        )
    {
        _userContextService = userContextService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleAuthorizationRequirement requirement)
    {
        var user = await _userContextService.GetCurrentContextAsync();
        var signedInUser = user.ToSignedInContext();

        if (signedInUser != null
            && signedInUser.UserArea.UserAreaCode == requirement.UserAreaCode
            && EnumerableHelper.Enumerate(requirement.RoleCodes).Contains(signedInUser.RoleCode))
        {
            context.Succeed(requirement);
        }
    }
}
