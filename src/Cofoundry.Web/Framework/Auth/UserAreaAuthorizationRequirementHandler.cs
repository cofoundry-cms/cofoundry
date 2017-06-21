using Cofoundry.Domain;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public class UserAreaAuthorizationHandler : AuthorizationHandler<UserAreaAuthorizationRequirement>
    {
        private readonly IUserContextService _userContextService;

        public UserAreaAuthorizationHandler(
            IUserContextService userContextService
            )
        {
            _userContextService = userContextService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            UserAreaAuthorizationRequirement requirement
            )
        {
            if (context.User?.Identity?.IsAuthenticated ?? false) return;

            var userContext = await _userContextService.GetCurrentContextAsync();

            if (userContext.UserId.HasValue && userContext.UserArea?.UserAreaCode == requirement.UserAreaCode)
            {
                context.Succeed(requirement);
            }
        }
    }
}