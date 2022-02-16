using Cofoundry.Core;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Domain.BackgroundTasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Internal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Marks a user as deleted in the database (soft delete), removing personal 
    /// data fields and any optional relations from the UnstructuredDataDependency
    /// table. The remaining user record and relations are left in place for auditing.
    /// Log tables that contain IP references are not deleted, but should be
    /// cleared out periodically by the <see cref="UserCleanupBackgroundTask"/>.
    /// </summary>
    public class DeleteUserCommandHandler
        : ICommandHandler<DeleteUserCommand>
        , IPermissionRestrictedCommandHandler<DeleteUserCommand>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IDomainRepository _domainRepository;
        private readonly IUserStoredProcedures _userStoredProcedures;
        private readonly UserCommandPermissionsHelper _userCommandPermissionsHelper;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly IUserContextCache _userContextCache;
        private readonly IMessageAggregator _messageAggregator;

        public DeleteUserCommandHandler(
            CofoundryDbContext dbContext,
            IDomainRepository domainRepository,
            IUserStoredProcedures userStoredProcedures,
            UserCommandPermissionsHelper userCommandPermissionsHelper,
            IPermissionValidationService permissionValidationService,
            IUserContextCache userContextCache,
            IMessageAggregator messageAggregator
            )
        {
            _dbContext = dbContext;
            _domainRepository = domainRepository;
            _userStoredProcedures = userStoredProcedures;
            _userCommandPermissionsHelper = userCommandPermissionsHelper;
            _permissionValidationService = permissionValidationService;
            _userContextCache = userContextCache;
            _messageAggregator = messageAggregator;
        }

        public async Task ExecuteAsync(DeleteUserCommand command, IExecutionContext executionContext)
        {
            var user = await GetUserAsync(command.UserId);
            await ValidateCustomPermissionsAsync(user, executionContext);

            // The may have already be soft deleted, but we should proceed anyway
            // to ensure data has been removed to the current specification
            var pseudonym = Guid.NewGuid().ToString("N");

            await _userStoredProcedures.SoftDeleteAsync(command.UserId, pseudonym, executionContext.ExecutionDate);
            await _domainRepository.Transactions().QueueCompletionTaskAsync(() => OnTransactionComplete(user));
        }

        private async Task OnTransactionComplete(User user)
        {
            _userContextCache.Clear(user.UserId);

            await _messageAggregator.PublishAsync(new UserDeletedMessage()
            {
                UserAreaCode = user.UserAreaCode,
                UserId = user.UserId
            });
        }

        private async Task<User> GetUserAsync(int userId)
        {
            var user = await _dbContext
                .Users
                .AsNoTracking()
                .Include(u => u.Role)
                .FilterById(userId)
                .SingleOrDefaultAsync();

            EntityNotFoundException.ThrowIfNull(user, userId);

            return user;
        }

        private async Task ValidateCustomPermissionsAsync(User user, IExecutionContext executionContext)
        {
            if (user.IsSystemAccount)
            {
                throw new NotPermittedException("You cannot delete the system account.");
            }

            if (user.UserAreaCode == CofoundryAdminUserArea.Code)
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
            await _userCommandPermissionsHelper.ThrowIfCannotManageSuperAdminAsync(user, executionContext);
        }

        public IEnumerable<IPermissionApplication> GetPermissions(DeleteUserCommand command)
        {
            yield return new CofoundryUserDeletePermission();
        }
    }
}