using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Data;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Deletes a role with the specified database id. Roles cannot be
    /// deleted if assigned to users.
    /// </summary>
    public class DeleteRoleCommandHandler 
        : ICommandHandler<DeleteRoleCommand>
        , IPermissionRestrictedCommandHandler<DeleteRoleCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IRoleCache _roleCache;
        private readonly ITransactionScopeManager _transactionScopeFactory;

        public DeleteRoleCommandHandler(
            CofoundryDbContext dbContext,
            UserCommandPermissionsHelper userCommandPermissionsHelper,
            IRoleCache roleCache,
            ITransactionScopeManager transactionScopeFactory
            )
        {
            _dbContext = dbContext;
            _roleCache = roleCache;
            _transactionScopeFactory = transactionScopeFactory;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(DeleteRoleCommand command, IExecutionContext executionContext)
        {
            var role = await _dbContext
                .Roles
                .FilterById(command.RoleId)
                .SingleOrDefaultAsync();

            if (role != null)
            {
                ValidateCanDelete(role, command);

                _dbContext.Roles.Remove(role);

                await _dbContext.SaveChangesAsync();
                _transactionScopeFactory.QueueCompletionTask(_dbContext, () => _roleCache.Clear(command.RoleId));
            }
        }

        private void ValidateCanDelete(Role role, DeleteRoleCommand command)
        {
            if (role.RoleCode == AnonymousRole.AnonymousRoleCode)
            {
                throw new ValidationException("The anonymous role cannot be deleted.");
            }

            if (role.RoleCode == SuperAdminRole.SuperAdminRoleCode)
            {
                throw new ValidationException("The super administrator role cannot be deleted.");
            }

            var isInUse = _dbContext
                .Users
                .Any(u => u.RoleId == command.RoleId);

            if (isInUse)
            {
                throw new ValidationException("Role is in use and cannot be deleted.");
            }
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(DeleteRoleCommand command)
        {
            yield return new RoleDeletePermission();
        }

        #endregion
    }
}
