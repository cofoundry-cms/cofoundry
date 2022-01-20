using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Internal;
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
        private readonly IUserStoredProcedures _userStoredProcedures;
        private readonly IDomainRepository _domainRepository;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly IUserUpdateCommandHelper _userUpdateCommandHelper;
        private readonly IUserSecurityStampUpdateHelper _userSecurityStampUpdateHelper;

        public UpdateCurrentUserCommandHandler(
            CofoundryDbContext dbContext,
            IUserStoredProcedures userStoredProcedures,
            IDomainRepository domainRepository,
            IPermissionValidationService permissionValidationService,
            IUserUpdateCommandHelper userUpdateCommandHelper,
            IUserSecurityStampUpdateHelper userSecurityStampUpdateHelper
            )
        {
            _dbContext = dbContext;
            _userStoredProcedures = userStoredProcedures;
            _domainRepository = domainRepository;
            _permissionValidationService = permissionValidationService;
            _userUpdateCommandHelper = userUpdateCommandHelper;
            _userSecurityStampUpdateHelper = userSecurityStampUpdateHelper;
        }

        public async Task ExecuteAsync(UpdateCurrentUserCommand command, IExecutionContext executionContext)
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

            if (updateResult.HasUpdate())
            {
                _userSecurityStampUpdateHelper.Update(user);
            }

            using (var scope = _domainRepository.Transactions().CreateScope())
            {
                await _dbContext.SaveChangesAsync();

                // Here we could assume that reset requests only need invalidating if the contact email changes, but if the
                // user is updating their account details, then we could also assume that old requests are stale anyway.
                await _userStoredProcedures.InvalidateUserAccountRecoveryRequests(userId, executionContext.ExecutionDate);

                scope.QueueCompletionTask(() => OnTransactionComplete(user, updateResult));
                await scope.CompleteAsync();
            }
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
