using Cofoundry.Core;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Marks a user as deleted in the database (soft delete). A soft delete is
    /// required because we can't realistically cascade deletions to all
    /// entities with auditing dependencies.
    /// </summary>
    public class DeleteUserCommandHandler
        : ICommandHandler<DeleteUserCommand>
        , IPermissionRestrictedCommandHandler<DeleteUserCommand>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IDomainRepository _domainRepository;
        private readonly UserCommandPermissionsHelper _userCommandPermissionsHelper;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly IUserContextCache _userContextCache;
        private readonly IMessageAggregator _messageAggregator;

        public DeleteUserCommandHandler(
            CofoundryDbContext dbContext,
            IDomainRepository domainRepository,
            UserCommandPermissionsHelper userCommandPermissionsHelper,
            IPermissionValidationService permissionValidationService,
            IUserContextCache userContextCache,
            IMessageAggregator messageAggregator
            )
        {
            _dbContext = dbContext;
            _domainRepository = domainRepository;
            _userCommandPermissionsHelper = userCommandPermissionsHelper;
            _permissionValidationService = permissionValidationService;
            _userContextCache = userContextCache;
            _messageAggregator = messageAggregator;
        }

        public async Task ExecuteAsync(DeleteUserCommand command, IExecutionContext executionContext)
        {
            var user = await QueryUser(command.UserId).SingleOrDefaultAsync();
            var executorRole = await _userCommandPermissionsHelper.GetExecutorRoleAsync(executionContext);
            ValidateCustomPermissions(user, executionContext, executorRole);
            MarkRecordDeleted(user, executionContext);

            using (var scope = _domainRepository.Transactions().CreateScope())
            {
                // Since we soft-delete, we have to validate/delete unstructured dependencies here rather than in the trigger
                await _domainRepository
                    .WithExecutionContext(executionContext)
                    .ExecuteCommandAsync(new DeleteUnstructuredDataDependenciesCommand(UserEntityDefinition.DefinitionCode, user.UserId));
                await _dbContext.SaveChangesAsync();

                scope.QueueCompletionTask(() => OnTransactionComplete(user));

                await scope.CompleteAsync();
            }
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
