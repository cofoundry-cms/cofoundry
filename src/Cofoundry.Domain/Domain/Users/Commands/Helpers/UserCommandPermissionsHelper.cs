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

        /// <summary>
        /// Throws a <see cref="NotPermittedException"/> if the user is attempting to update
        /// or delete a Cofoundry super admin account from a less privelidged account.
        /// </summary>
        /// <param name="user">User being managed.</param>
        /// <param name="executionContext">The execution context with information on the current user.</param>
        /// <param name="message">The exception message to throw if invalid.</param>
        public async Task ThrowIfCannotManageSuperAdminAsync(
            User user, 
            IExecutionContext executionContext
            )
        {
            var userContext = executionContext.UserContext;
            if (!userContext.IsSignedIn()) throw new InvalidOperationException("User expected to be signed in to do role management validation");

            if (userContext.IsSuperAdmin()) return;

            var userRole = await  _internalRoleRepository.GetByIdAsync(user.RoleId);

            if (userRole.IsSuperAdminRole)
            {
                throw new NotPermittedException("Only Super Administrator users can manage other users with the Super Administrator role");
            }
        }

        public async Task ValidateNewRoleAsync(Role newRole, int? oldRoleId, string userAreaCode, IExecutionContext executionContext)
        {
            if (newRole == null) throw new ArgumentNullException(nameof(newRole));
            if (userAreaCode == null) throw new ArgumentNullException(nameof(userAreaCode));
            if (executionContext == null) throw new ArgumentNullException(nameof(executionContext));

            var executorRole = await GetExecutorRoleAsync(executionContext);

            ValidateNewRole(userAreaCode, newRole, executorRole);

            await ValidateRoleDeAssignmentAsync(oldRoleId, newRole, executorRole);
        }

        public async Task<RoleDetails> GetExecutorRoleAsync(IExecutionContext executionContext)
        {
            var executorRole = await _internalRoleRepository.GetByIdAsync(executionContext.UserContext.RoleId);

            return executorRole;
        }

        private void ValidateNewRole(string userAreaCode, Role newUserRole, RoleDetails executorRole)
        {
            // Anonymous role is not assignable to users, it's used when there is no user.
            if (newUserRole.IsAnonymousRole())
            {
                throw new ValidationErrorException("Cannot assign the anonymous role.");
            }

            if (userAreaCode != newUserRole.UserAreaCode)
            {
                throw new ValidationErrorException("Cannot assign a role from one user area to a user from a different user area.");
            }

            // Cofoundry Admin Users can assign any roles, otherwise the user has to be in the same user area
            if (newUserRole.UserAreaCode != userAreaCode && executorRole.UserArea.UserAreaCode != CofoundryAdminUserArea.Code)
            {
                throw new NotPermittedException("You are not permitted to assign this role.");
            }

            // Only super admins can assign the super admin role
            if (newUserRole.IsSuperAdminRole() && !executorRole.IsSuperAdminRole)
            {
                throw new NotPermittedException("Only Super Administrator users can assign the Super Administrator role");
            }
        }

        private async Task ValidateRoleDeAssignmentAsync(int? oldRoleId, Role newUserRole, RoleDetails executorRole)
        {
            if (oldRoleId.HasValue
                && !executorRole.IsSuperAdminRole
                && newUserRole.RoleCode != SuperAdminRole.Code)
            {
                var oldRole = await _dbContext
                    .Roles
                    .FilterById(oldRoleId.Value)
                    .SingleOrDefaultAsync();

                if (oldRole.RoleCode == SuperAdminRole.Code)
                {
                    throw new NotPermittedException("Only Super Administrator users can de-assign the Super Administrator role");
                }
            }
        }
    }
}
