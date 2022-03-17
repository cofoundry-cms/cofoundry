using Cofoundry.Domain.Internal;
using Microsoft.AspNetCore.Authorization;

namespace Cofoundry.Web.Auth.Internal;

/// <summary>
/// Handles the authorization of the <see cref="PermissionAuthorizationRequirement"/> in
/// an authorization policy.
/// </summary>
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionAuthorizationRequirement>
{
    private readonly IUserContextService _userContextService;
    private readonly IInternalRoleRepository _internalRoleRepository;

    public PermissionAuthorizationHandler(
        IUserContextService userContextService,
        IInternalRoleRepository internalRoleRepository
        )
    {
        _userContextService = userContextService;
        _internalRoleRepository = internalRoleRepository;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionAuthorizationRequirement requirement)
    {
        var user = await _userContextService.GetCurrentContextAsync();
        var role = await _internalRoleRepository.GetByIdAsync(user.RoleId);

        if (role != null && requirement.Permission != null && role.HasPermission(requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}
