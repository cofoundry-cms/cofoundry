using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
{
    public class UserCommandPermissionsHelper
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IInternalRoleRepository _internalRoleRepository;
        private readonly IUserAreaDefinitionRepository _userAreaRepository;

        public UserCommandPermissionsHelper(
            CofoundryDbContext dbContext,
            IInternalRoleRepository internalRoleRepository,
            IUserAreaDefinitionRepository userAreaRepository
            )
        {
            _dbContext = dbContext;
            _internalRoleRepository = internalRoleRepository;
            _userAreaRepository = userAreaRepository;
        }

        #endregion

        #region public methods

        public async Task<Role> GetAndValidateNewRoleAsync(int newRoleId, int? oldRoleId, string userAreaCode, IExecutionContext executionContext)
        {
            var executorRole = await GetExecutorRoleAsync(executionContext);

            var newRole = await QueryRole(newRoleId).SingleOrDefaultAsync();
            
            EntityNotFoundException.ThrowIfNull(newRole, newRoleId);
            ValidateRole(userAreaCode, newRole, executorRole);

            await ValidateDeAssignmentAsync(oldRoleId, newRole, executorRole);

            return newRole;
        }

        public async Task<RoleDetails> GetExecutorRoleAsync(IExecutionContext executionContext)
        {
            var executorRole = await _internalRoleRepository.GetByIdAsync(executionContext.UserContext.RoleId);

            return executorRole;
        }

        #endregion

        #region private helpers

        private void ValidateRole(string userAreaCode, Role newUserRole, RoleDetails executorRole)
        {
            // Anonymous role is not assignable to users, it's used when there is no user.
            if (newUserRole.RoleCode == AnonymousRole.AnonymousRoleCode)
            {
                throw new NotPermittedException("Cannot assign the anonymous role.");
            }

            if (userAreaCode != newUserRole.UserAreaCode)
            {
                throw new NotPermittedException("Cannot assign a role from one user area to a user from a different user area.");
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
                var oldRole = await QueryRole(oldRoleId.Value).SingleOrDefaultAsync();
                if (oldRole.RoleCode == SuperAdminRole.SuperAdminRoleCode)
                {
                    throw new NotPermittedException("Only Super Administrator users can de-assign the Super Administrator role");
                }
            }
        }

        private IQueryable<Role> QueryRole(int roleId)
        {
            return _dbContext
                .Roles
                .FilterById(roleId);
        }

        #endregion
    }
}
