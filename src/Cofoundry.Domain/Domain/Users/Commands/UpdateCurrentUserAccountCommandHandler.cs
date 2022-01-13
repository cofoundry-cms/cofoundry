using Cofoundry.Core;
using Cofoundry.Core.Data;
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
    public class UpdateCurrentUserAccountCommandHandler
        : ICommandHandler<UpdateCurrentUserAccountCommand>
        , IPermissionRestrictedCommandHandler<UpdateCurrentUserAccountCommand>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly ITransactionScopeManager _transactionScopeManager;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly IUserUpdateCommandHelper _userUpdateCommandHelper;
        private readonly IUserSecurityStampUpdateHelper _userSecurityStampUpdateHelper;

        public UpdateCurrentUserAccountCommandHandler(
            CofoundryDbContext dbContext,
            ITransactionScopeManager transactionScopeManager,
            IPermissionValidationService permissionValidationService,
            IUserUpdateCommandHelper userUpdateCommandHelper,
            IUserSecurityStampUpdateHelper userSecurityStampUpdateHelper
            )
        {
            _dbContext = dbContext;
            _transactionScopeManager = transactionScopeManager;
            _permissionValidationService = permissionValidationService;
            _userUpdateCommandHelper = userUpdateCommandHelper;
            _userSecurityStampUpdateHelper = userSecurityStampUpdateHelper;
        }

        public async Task ExecuteAsync(UpdateCurrentUserAccountCommand command, IExecutionContext executionContext)
        {
            _permissionValidationService.EnforceIsLoggedIn(executionContext.UserContext);
            var userId = executionContext.UserContext.UserId.Value;

            var user = await _dbContext
                .Users
                .FilterCanLogIn()
                .FilterById(userId)
                .SingleOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(user, userId);

            var updateResult = await _userUpdateCommandHelper.UpdateEmailAndUsernameAsync(command.Email, command.Username, user, executionContext);
            user.FirstName = command.FirstName?.Trim();
            user.LastName = command.LastName?.Trim();

            if (updateResult.HasEmailChanged || updateResult.HasUsernameChanged)
            {
                _userSecurityStampUpdateHelper.Update(user);
            }

            await _dbContext.SaveChangesAsync();
            await _transactionScopeManager.QueueCompletionTaskAsync(_dbContext, () => OnTransactionComplete(user, updateResult));
        }

        private async Task OnTransactionComplete(User user, UserUpdateCommandHelper.UpdateEmailAndUsernameResult updateResult)
        {
            if (updateResult.HasEmailChanged || updateResult.HasUsernameChanged)
            {
                await _userSecurityStampUpdateHelper.OnTransactionCompleteAsync(user);
            }

            await _userUpdateCommandHelper.PublishUpdateMessagesAsync(user, updateResult);
        }

        public IEnumerable<IPermissionApplication> GetPermissions(UpdateCurrentUserAccountCommand command)
        {
            yield return new CurrentUserUpdatePermission();
        }
    }
}
