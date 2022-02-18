using Cofoundry.Core;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// A generic user update command for use with Cofoundry users and
    /// other non-Cofoundry users.
    /// </summary>
    public class UpdateUserCommandHandler
        : ICommandHandler<UpdateUserCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IDomainRepository _domainRepository;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly UserCommandPermissionsHelper _userCommandPermissionsHelper;
        private readonly IUserAreaDefinitionRepository _userAreaRepository;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly IUserContextCache _userContextCache;
        private readonly IUserUpdateCommandHelper _userUpdateCommandHelper;
        private readonly IUserSecurityStampUpdateHelper _userSecurityStampUpdateHelper;
        private readonly IMessageAggregator _messageAggregator;

        public UpdateUserCommandHandler(
            CofoundryDbContext dbContext,
            IDomainRepository domainRepository,
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            UserCommandPermissionsHelper userCommandPermissionsHelper,
            IUserAreaDefinitionRepository userAreaRepository,
            IPermissionValidationService permissionValidationService,
            IUserContextCache userContextCache,
            IUserUpdateCommandHelper userUpdateCommandHelper,
            IUserSecurityStampUpdateHelper userSecurityStampUpdateHelper,
            IMessageAggregator messageAggregator
            )
        {
            _dbContext = dbContext;
            _domainRepository = domainRepository;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _userCommandPermissionsHelper = userCommandPermissionsHelper;
            _userAreaRepository = userAreaRepository;
            _permissionValidationService = permissionValidationService;
            _userContextCache = userContextCache;
            _userUpdateCommandHelper = userUpdateCommandHelper;
            _userSecurityStampUpdateHelper = userSecurityStampUpdateHelper;
            _messageAggregator = messageAggregator;
        }

        public async Task ExecuteAsync(UpdateUserCommand command, IExecutionContext executionContext)
        {
            var user = await GetUserAsync(command);

            var userArea = _userAreaRepository.GetRequiredByCode(user.UserAreaCode);
            await ValidatePermissionsAsync(user, userArea, executionContext);

            var updateStatus = await UpdateEmailAndUsernameAsync(command, user, executionContext);
            await UpdateRoleAsync(command, executionContext, user);
            UpdateAccountVerifiedStatus(command, user, updateStatus, executionContext);
            UpdateActivationStatus(command, user, updateStatus, executionContext);
            UpdateProperties(command, user);

            if (updateStatus.RequiresSecurityStampUpdate())
            {
                _userSecurityStampUpdateHelper.Update(user);
            }

            using (var scope = _domainRepository.Transactions().CreateScope())
            {
                await _dbContext.SaveChangesAsync();
                await InvalidateAuthorizedTasks(user, updateStatus, executionContext);

                scope.QueueCompletionTask(() => OnTransactionComplete(user, updateStatus));
                await scope.CompleteAsync();
            }
        }

        private async Task OnTransactionComplete(
            User user,
            UpdateStatus updateStatus
            )
        {
            _userContextCache.Clear(user.UserId);

            if (updateStatus.RequiresSecurityStampUpdate())
            {
                await _userSecurityStampUpdateHelper.OnTransactionCompleteAsync(user);
            }

            if (updateStatus.HasActivationStatusChanged)
            {
                await _messageAggregator.PublishAsync(new UserActivationStatusUpdatedMessage()
                {
                    UserAreaCode = user.UserAreaCode,
                    UserId = user.UserId,
                    IsActive = !user.DeactivatedDate.HasValue
                });
            }

            if (updateStatus.HasVerificationStatusChanged)
            {
                await _messageAggregator.PublishAsync(new UserAccountVerificationStatusUpdatedMessage()
                {
                    UserAreaCode = user.UserAreaCode,
                    UserId = user.UserId,
                    IsVerified = user.AccountVerifiedDate.HasValue
                });
            }

            await _userUpdateCommandHelper.PublishUpdateMessagesAsync(user, updateStatus.UpdateEmailAndUsernameResult);
        }

        private async Task<User> GetUserAsync(UpdateUserCommand command)
        {
            var user = await _dbContext
                .Users
                .FilterNotDeleted()
                .FilterNotSystemAccount()
                .FilterById(command.UserId)
                .SingleOrDefaultAsync();

            EntityNotFoundException.ThrowIfNull(user, command.UserId);

            return user;
        }

        public async Task ValidatePermissionsAsync(User user, IUserAreaDefinition userArea, IExecutionContext executionContext)
        {
            if (userArea is CofoundryAdminUserArea)
            {
                _permissionValidationService.EnforcePermission(new CofoundryUserUpdatePermission(), executionContext.UserContext);
            }
            else
            {
                _permissionValidationService.EnforcePermission(new NonCofoundryUserUpdatePermission(), executionContext.UserContext);
            }

            await _userCommandPermissionsHelper.ThrowIfCannotManageSuperAdminAsync(user, executionContext);
        }

        private async Task<UpdateStatus> UpdateEmailAndUsernameAsync(
            UpdateUserCommand command,
            User user,
            IExecutionContext executionContext
            )
        {
            var updateEmaiAndUsernameResult = await _userUpdateCommandHelper.UpdateEmailAndUsernameAsync(command.Email, command.Username, user, executionContext);
            var updateStatus = new UpdateStatus(updateEmaiAndUsernameResult);

            return updateStatus;
        }

        private async Task UpdateRoleAsync(UpdateUserCommand command, IExecutionContext executionContext, User user)
        {
            // if a code is supplied we assume we're updating the role, otherwise check the id has changed
            if (!string.IsNullOrWhiteSpace(command.RoleCode)
                || (command.RoleId.HasValue && command.RoleId != user.RoleId))
            {
                var newRole = await _dbContext
                      .Roles
                      .FilterByIdOrCode(command.RoleId, command.RoleCode)
                      .SingleOrDefaultAsync();
                EntityNotFoundException.ThrowIfNull(newRole, command.RoleId?.ToString() ?? command.RoleCode);

                await _userCommandPermissionsHelper.ValidateNewRoleAsync(
                    newRole,
                    user.RoleId,
                    user.UserAreaCode,
                    executionContext
                    );

                user.Role = newRole;
            }
        }

        private async Task InvalidateAuthorizedTasks(
            User user,
            UpdateStatus updateStatus,
            IExecutionContext executionContext
            )
        {
            InvalidateAuthorizedTaskBatchCommand command = null;

            if (updateStatus.HasBeenDeactivated)
            {
                command = new InvalidateAuthorizedTaskBatchCommand(user.UserId);
            }
            else if (updateStatus.HasEmailChanged)
            {
                // The only reason to invalidate would be if the contact email that a request was sent to was changed
                command = new InvalidateAuthorizedTaskBatchCommand(user.UserId, UserAccountRecoveryAuthorizedTaskType.Code);
            }

            if (command != null)
            {
                await _domainRepository
                    .WithContext(executionContext)
                    .ExecuteCommandAsync(command);
            }
        }

        private void UpdateProperties(UpdateUserCommand command, User user)
        {
            var options = _userAreaDefinitionRepository.GetOptionsByCode(user.UserAreaCode);
            if (options.Username.UseAsDisplayName)
            {
                user.DisplayName = user.Username;
            }
            else
            {
                user.DisplayName = command.DisplayName?.Trim();
            }

            user.FirstName = command.FirstName?.Trim();
            user.LastName = command.LastName?.Trim();
            user.RequirePasswordChange = command.RequirePasswordChange;
        }

        private static void UpdateAccountVerifiedStatus(
            UpdateUserCommand command,
            User user,
            UpdateStatus updateStatus,
            IExecutionContext executionContext
            )
        {
            updateStatus.HasVerificationStatusChanged = user.AccountVerifiedDate.HasValue != command.IsAccountVerified;

            if (!updateStatus.HasVerificationStatusChanged) return;

            if (command.IsAccountVerified)
            {
                user.AccountVerifiedDate = executionContext.ExecutionDate;
            }
            else
            {
                user.AccountVerifiedDate = null;
            }
        }

        private static void UpdateActivationStatus(
            UpdateUserCommand command, 
            User user, 
            UpdateStatus updateStatus,
            IExecutionContext executionContext
            )
        {
            var isCurrentlyActive = !user.DeactivatedDate.HasValue;
            updateStatus.HasActivationStatusChanged = isCurrentlyActive != command.IsActive;
            if (!updateStatus.HasActivationStatusChanged) return;

            if (!command.IsActive && user.UserId == executionContext.UserContext.UserId)
            {
                throw new NotPermittedException("A user cannot deactivate their own account.");
            }

            if (command.IsActive)
            {
                user.DeactivatedDate = null;
            }
            else
            {
                user.DeactivatedDate = executionContext.ExecutionDate;
            }

            updateStatus.HasBeenDeactivated = !command.IsActive;
        }

        private class UpdateStatus
        {
            private readonly UserUpdateCommandHelper.UpdateEmailAndUsernameResult _updateEmailAndUsernameResult;

            public UpdateStatus(UserUpdateCommandHelper.UpdateEmailAndUsernameResult updateEmailAndUsernameResult)
            {
                _updateEmailAndUsernameResult = updateEmailAndUsernameResult;
            }

            public bool HasEmailChanged => _updateEmailAndUsernameResult.HasEmailChanged;

            public bool HasUsernameChanged => _updateEmailAndUsernameResult.HasUsernameChanged;

            public bool HasVerificationStatusChanged { get; set; }

            public bool HasBeenDeactivated { get; set; }

            public bool HasActivationStatusChanged { get; set; }

            public UserUpdateCommandHelper.UpdateEmailAndUsernameResult UpdateEmailAndUsernameResult => _updateEmailAndUsernameResult;

            public bool RequiresSecurityStampUpdate()
            {
                return _updateEmailAndUsernameResult.HasUpdate() || HasBeenDeactivated;
            }
        }
    }
}
