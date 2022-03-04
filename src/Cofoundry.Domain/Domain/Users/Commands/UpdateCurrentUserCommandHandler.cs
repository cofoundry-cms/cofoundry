using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Updates the user account of the currently logged in user.
    /// </summary>
    public class UpdateCurrentUserCommandHandler
        : ICommandHandler<UpdateCurrentUserCommand>
        , IPermissionRestrictedCommandHandler<UpdateCurrentUserCommand>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IDomainRepository _domainRepository;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly IUserUpdateCommandHelper _userUpdateCommandHelper;
        private readonly IUserSecurityStampUpdateHelper _userSecurityStampUpdateHelper;

        public UpdateCurrentUserCommandHandler(
            CofoundryDbContext dbContext,
            IDomainRepository domainRepository,
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IPermissionValidationService permissionValidationService,
            IUserUpdateCommandHelper userUpdateCommandHelper,
            IUserSecurityStampUpdateHelper userSecurityStampUpdateHelper
            )
        {
            _dbContext = dbContext;
            _domainRepository = domainRepository;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _permissionValidationService = permissionValidationService;
            _userUpdateCommandHelper = userUpdateCommandHelper;
            _userSecurityStampUpdateHelper = userSecurityStampUpdateHelper;
        }

        public async Task ExecuteAsync(UpdateCurrentUserCommand command, IExecutionContext executionContext)
        {
            _permissionValidationService.EnforceIsSignedIn(executionContext.UserContext);
            var userId = executionContext.UserContext.UserId.Value;

            var user = await _dbContext
                .Users
                .FilterCanSignIn()
                .FilterById(userId)
                .SingleOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(user, userId);

            var updateResult = await _userUpdateCommandHelper.UpdateEmailAndUsernameAsync(command.Email, command.Username, user, executionContext);
            UpdateName(command, user);
            user.FirstName = command.FirstName?.Trim();
            user.LastName = command.LastName?.Trim();

            if (updateResult.HasUpdate())
            {
                _userSecurityStampUpdateHelper.Update(user);
            }

            using (var scope = _domainRepository.Transactions().CreateScope())
            {
                await _dbContext.SaveChangesAsync();

                // Here we could assume that reset requests only need invalidating if the contact email changes, but if the
                // user is updating their account details, then we could also assume that old requests are stale anyway.
                await _domainRepository
                    .WithContext(executionContext)
                    .ExecuteCommandAsync(new InvalidateAuthorizedTaskBatchCommand(userId, UserAccountRecoveryAuthorizedTaskType.Code));

                scope.QueueCompletionTask(() => OnTransactionComplete(user, updateResult));
                await scope.CompleteAsync();
            }
        }

        private void UpdateName(UpdateCurrentUserCommand command, User user)
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
        }

        private async Task OnTransactionComplete(User user, UserUpdateCommandHelper.UpdateEmailAndUsernameResult updateResult)
        {
            if (updateResult.HasUpdate())
            {
                await _userSecurityStampUpdateHelper.OnTransactionCompleteAsync(user);
            }

            await _userUpdateCommandHelper.PublishUpdateMessagesAsync(user, updateResult);
        }

        public IEnumerable<IPermissionApplication> GetPermissions(UpdateCurrentUserCommand command)
        {
            yield return new CurrentUserUpdatePermission();
        }
    }
}
