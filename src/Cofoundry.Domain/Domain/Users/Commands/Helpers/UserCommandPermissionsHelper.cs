using Cofoundry.Core;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class UserCommandPermissionsHelper
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IInternalRoleRepository _internalRoleRepository;

        public UserCommandPermissionsHelper(
            CofoundryDbContext dbContext,
            IInternalRoleRepository internalRoleRepository
            )
        {
            _dbContext = dbContext;
            _internalRoleRepository = internalRoleRepository;
        }

        public async Task ValidateNewRoleAsync(Role newRole, int? oldRoleId, string userAreaCode, IExecutionContext executionContext)
        {
            if (newRole == null) throw new ArgumentNullException(nameof(newRole));
            if (userAreaCode == null) throw new ArgumentNullException(nameof(userAreaCode));
            if (executionContext == null) throw new ArgumentNullException(nameof(executionContext));

            var executorRole = await GetExecutorRoleAsync(executionContext);

            ValidateRole(userAreaCode, newRole, executorRole);

            await ValidateDeAssignmentAsync(oldRoleId, newRole, executorRole);
        }

        public async Task<RoleDetails> GetExecutorRoleAsync(IExecutionContext executionContext)
        {
            var executorRole = await _internalRoleRepository.GetByIdAsync(executionContext.UserContext.RoleId);

            return executorRole;
        }

        private void ValidateRole(string userAreaCode, Role newUserRole, RoleDetails executorRole)
        {
            // Anonymous role is not assignable to users, it's used when there is no user.
            if (newUserRole.RoleCode == AnonymousRole.AnonymousRoleCode)
            {
                throw new ValidationErrorException("Cannot assign the anonymous role.");
            }

            if (userAreaCode != newUserRole.UserAreaCode)
            {
                throw new ValidationErrorException("Cannot assign a role from one user area to a user from a different user area.");
            }


            // Cofoundry Admin Users can assign any roles, otherwise the user has to be in the same user area
            if (newUserRole.UserAreaCode != userAreaCode && executorRole.UserArea.UserAreaCode != CofoundryAdminUserArea.AreaCode)
            {
                throw new NotPermittedException("You are not permitted to assign this role.");
            }

            // Only super admins can assign the super admin role
            if (newUserRole.RoleCode == SuperAdminRole.SuperAdminRoleCode && !executorRole.IsSuperAdministrator)
            {
                throw new NotPermittedException("Only Super Administrator users can assign the Super Administrator role");
            }
        }

        private async Task ValidateDeAssignmentAsync(int? oldRoleId, Role newUserRole, RoleDetails executorRole)
        {
            if (oldRoleId.HasValue
                && !executorRole.IsSuperAdministrator
                && newUserRole.RoleCode != SuperAdminRole.SuperAdminRoleCode)
            {
                var oldRole = await _dbContext
                    .Roles
                    .FilterById(oldRoleId.Value)
                    .SingleOrDefaultAsync();

                if (oldRole.RoleCode == SuperAdminRole.SuperAdminRoleCode)
                {
                    throw new NotPermittedException("Only Super Administrator users can de-assign the Super Administrator role");
                }
            }
        }
    }
}
