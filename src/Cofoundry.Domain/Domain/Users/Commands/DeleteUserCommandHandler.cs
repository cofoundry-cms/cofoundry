using Cofoundry.Core;
using Cofoundry.Core.Data;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Marks a user as deleted in the database (soft delete).
    /// </summary>
    public class DeleteUserCommandHandler
        : ICommandHandler<DeleteUserCommand>
        , IPermissionRestrictedCommandHandler<DeleteUserCommand>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly UserCommandPermissionsHelper _userCommandPermissionsHelper;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly ITransactionScopeManager _transactionScopeManager;
        private readonly IUserContextCache _userContextCache;

        public DeleteUserCommandHandler(
            CofoundryDbContext dbContext,
            UserCommandPermissionsHelper userCommandPermissionsHelper,
            IPermissionValidationService permissionValidationService,
            ITransactionScopeManager transactionScopeManager,
            IUserContextCache userContextCache
            )
        {
            _dbContext = dbContext;
            _userCommandPermissionsHelper = userCommandPermissionsHelper;
            _permissionValidationService = permissionValidationService;
            _transactionScopeManager = transactionScopeManager;
            _userContextCache = userContextCache;
        }

        public async Task ExecuteAsync(DeleteUserCommand command, IExecutionContext executionContext)
        {
            var user = await QueryUser(command.UserId).SingleOrDefaultAsync();
            var executorRole = await _userCommandPermissionsHelper.GetExecutorRoleAsync(executionContext);
            ValidateCustomPermissions(user, executionContext, executorRole);
            MarkRecordDeleted(user, executionContext);

            await _dbContext.SaveChangesAsync();
            _transactionScopeManager.QueueCompletionTask(_dbContext, () => _userContextCache.Clear(command.UserId));
        }

        private void MarkRecordDeleted(User user, IExecutionContext executionContext)
        {
            if (user != null && !user.IsDeleted)
            {
                user.IsDeleted = true;
            }
        }

        private IQueryable<User> QueryUser(int userId)
        {
            return _dbContext
                .Users
                .Include(u => u.Role)
                .FilterById(userId);
        }

        private void ValidateCustomPermissions(User user, IExecutionContext executionContext, RoleDetails executorRole)
        {
            if (user.IsSystemAccount)
            {
                throw new NotPermittedException("You cannot delete the system account.");
            }
            if (user.UserAreaCode == CofoundryAdminUserArea.AreaCode)
            {
                _permissionValidationService.EnforcePermission(new CofoundryUserUpdatePermission(), executionContext.UserContext);
            }
            else
            {
                _permissionValidationService.EnforcePermission(new NonCofoundryUserUpdatePermission(), executionContext.UserContext);
            }

            if (user.UserId == executionContext.UserContext.UserId)
            {
                throw new NotPermittedException("You cannot delete your own user account via this api.");
            }

            // Only super admins can delete super admin
            if (user.Role.RoleCode == SuperAdminRole.SuperAdminRoleCode && !executorRole.IsSuperAdministrator)
            {
                throw new NotPermittedException("Only Super Administrator users can delete other users with the Super Administrator role");
            }
        }

        public IEnumerable<IPermissionApplication> GetPermissions(DeleteUserCommand command)
        {
            yield return new CofoundryUserDeletePermission();
        }
    }
}
