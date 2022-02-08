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
            var user = await _dbContext
                .Users
                .FilterCanSignIn()
                .FilterById(command.UserId)
                .SingleOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(user, command.UserId);

            var userArea = _userAreaRepository.GetRequiredByCode(user.UserAreaCode);
            ValidatePermissions(userArea, executionContext);

            await UpdateRoleAsync(command, executionContext, user);
            var updateEmaiAndUsernameResult = await _userUpdateCommandHelper.UpdateEmailAndUsernameAsync(command.Email, command.Username, user, executionContext);
            var hasVerificationStatusChanged = UpdateAccountVerifiedStatus(command, user, executionContext);

            UpdateProperties(command, user);

            if (updateEmaiAndUsernameResult.HasUpdate())
            {
                _userSecurityStampUpdateHelper.Update(user);
            }

            using (var scope = _domainRepository.Transactions().CreateScope())
            {
                await _dbContext.SaveChangesAsync();
                if (updateEmaiAndUsernameResult.HasEmailChanged)
                {
                    // The only reason to invalidate would be if the contact email that a request was sent to was changed
                    await _domainRepository
                        .WithContext(executionContext)
                        .ExecuteCommandAsync(new InvalidateAuthorizedTaskBatchCommand(user.UserId, UserAccountRecoveryAuthorizedTaskType.Code));
                }

                scope.QueueCompletionTask(() => OnTransactionComplete(user, updateEmaiAndUsernameResult, hasVerificationStatusChanged));
                await scope.CompleteAsync();
            }
        }

        private async Task OnTransactionComplete(
            User user, 
            UserUpdateCommandHelper.UpdateEmailAndUsernameResult updateResult,
            bool hasVerificationStatusChanged)
        {
            _userContextCache.Clear(user.UserId);

            if (updateResult.HasUpdate())
            {
                await _userSecurityStampUpdateHelper.OnTransactionCompleteAsync(user);
            }

            if (hasVerificationStatusChanged)
            {
                await _messageAggregator.PublishAsync(new UserAccountVerificationStatusUpdatedMessage()
                {
                    UserAreaCode = user.UserAreaCode,
                    UserId = user.UserId,
                    IsVerified = user.AccountVerifiedDate.HasValue
                });
            }

            await _userUpdateCommandHelper.PublishUpdateMessagesAsync(user, updateResult);
        }

        public void ValidatePermissions(IUserAreaDefinition userArea, IExecutionContext executionContext)
        {
            if (userArea is CofoundryAdminUserArea)
            {
                _permissionValidationService.EnforcePermission(new CofoundryUserUpdatePermission(), executionContext.UserContext);
            }
            else
            {
                _permissionValidationService.EnforcePermission(new NonCofoundryUserUpdatePermission(), executionContext.UserContext);
            }
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

        private static void UpdateProperties(UpdateUserCommand command, User user)
        {
            user.FirstName = command.FirstName?.Trim();
            user.LastName = command.LastName?.Trim();
            user.RequirePasswordChange = command.RequirePasswordChange;
        }

        private static bool UpdateAccountVerifiedStatus(UpdateUserCommand command, User user, IExecutionContext executionContext)
        {
            var hasChanged = user.AccountVerifiedDate.HasValue != command.IsAccountVerified;
            if (!hasChanged) return hasChanged;

            if (command.IsAccountVerified)
            {
                user.AccountVerifiedDate = executionContext.ExecutionDate;
            }
            else
            {
                user.AccountVerifiedDate = null;
            }

            return hasChanged;
        }
    }
}
