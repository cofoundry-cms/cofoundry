using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class DeleteUserCommandHandler 
        : ICommandHandler<DeleteUserCommand>, IAsyncCommandHandler<DeleteUserCommand>
        , IPermissionRestrictedCommandHandler<DeleteUserCommand>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly UserCommandPermissionsHelper _userCommandPermissionsHelper;
        private readonly IPermissionValidationService _permissionValidationService;

        public DeleteUserCommandHandler(
            CofoundryDbContext dbContext,
            UserCommandPermissionsHelper userCommandPermissionsHelper,
            IPermissionValidationService permissionValidationService
            )
        {
            _dbContext = dbContext;
            _userCommandPermissionsHelper = userCommandPermissionsHelper;
            _permissionValidationService = permissionValidationService;
        }

        public void Execute(DeleteUserCommand command, IExecutionContext executionContext)
        {
            var user = QueryUser(command.UserId).SingleOrDefault();
            var executorRole = _userCommandPermissionsHelper.GetExecutorRole(executionContext);
            ValidateCustomPermissions(user, executionContext, executorRole);
            MarkRecordDeleted(user, executionContext);
            _dbContext.SaveChanges();
        }

        public async Task ExecuteAsync(DeleteUserCommand command, IExecutionContext executionContext)
        {
            var user = await QueryUser(command.UserId).SingleOrDefaultAsync();
            var executorRole = await _userCommandPermissionsHelper.GetExecutorRoleAsync(executionContext);
            ValidateCustomPermissions(user, executionContext, executorRole);
            MarkRecordDeleted(user, executionContext);
            await _dbContext.SaveChangesAsync();
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

        #region Permissions

        private void ValidateCustomPermissions(User user, IExecutionContext executionContext, RoleDetails executorRole)
        {
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
            if (user.Role.SpecialistRoleTypeCode == SpecialistRoleTypeCodes.SuperAdministrator && !executorRole.IsSuperAdministrator)
            {
                throw new NotPermittedException("Only Super Administrator users can delete other users with the Super Administrator role");
            }
        }

        public IEnumerable<IPermissionApplication> GetPermissions(DeleteUserCommand command)
        {
            yield return new CofoundryUserDeletePermission();
        }

        #endregion
    }
}
