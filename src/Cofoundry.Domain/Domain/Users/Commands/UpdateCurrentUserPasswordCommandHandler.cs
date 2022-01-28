using Cofoundry.Core;
using Cofoundry.Core.Data;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Updates the password of the currently logged in user, using the
    /// <see cref="UpdateCurrentUserPasswordCommand.OldPassword"/> field 
    /// to authenticate the request.
    /// </summary>
    public class UpdateCurrentUserPasswordCommandHandler
        : ICommandHandler<UpdateCurrentUserPasswordCommand>
        , IPermissionRestrictedCommandHandler<UpdateCurrentUserPasswordCommand>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IUserStoredProcedures _userStoredProcedures;
        private readonly IDomainRepository _domainRepository;
        private readonly UserAuthenticationHelper _userAuthenticationHelper;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly IUserAreaDefinitionRepository _userAreaRepository;
        private readonly IPasswordUpdateCommandHelper _passwordUpdateCommandHelper;
        private readonly IUserSecurityStampUpdateHelper _userSecurityStampUpdateHelper;
        private readonly IUserContextCache _userContextCache;
        private readonly IPasswordPolicyService _newPasswordValidationService;
        private readonly IMessageAggregator _messageAggregator;

        public UpdateCurrentUserPasswordCommandHandler(
            CofoundryDbContext dbContext,
            IUserStoredProcedures userStoredProcedures,
            IDomainRepository domainRepository,
            UserAuthenticationHelper userAuthenticationHelper,
            IPermissionValidationService permissionValidationService,
            IUserAreaDefinitionRepository userAreaRepository,
            IPasswordUpdateCommandHelper passwordUpdateCommandHelper,
            IUserSecurityStampUpdateHelper userSecurityStampUpdateHelper,
            IUserContextCache userContextCache,
            IPasswordPolicyService newPasswordValidationService,
            IMessageAggregator messageAggregator
            )
        {
            _dbContext = dbContext;
            _userStoredProcedures = userStoredProcedures;
            _domainRepository = domainRepository;
            _userAuthenticationHelper = userAuthenticationHelper;
            _permissionValidationService = permissionValidationService;
            _userAreaRepository = userAreaRepository;
            _passwordUpdateCommandHelper = passwordUpdateCommandHelper;
            _userSecurityStampUpdateHelper = userSecurityStampUpdateHelper;
            _userContextCache = userContextCache;
            _newPasswordValidationService = newPasswordValidationService;
            _messageAggregator = messageAggregator;
        }

        public async Task ExecuteAsync(UpdateCurrentUserPasswordCommand command, IExecutionContext executionContext)
        {
            _permissionValidationService.EnforceIsLoggedIn(executionContext.UserContext);

            var user = await GetUser(executionContext);
            EntityNotFoundException.ThrowIfNull(user, executionContext.UserContext.UserId);

            await ValidateMaxLoginAttemptsNotExceededAsync(user, executionContext);
            await AuthenticateAsync(command, user);
            await ValidatePasswordAsync(command, user, executionContext);

            _passwordUpdateCommandHelper.UpdatePassword(command.NewPassword, user, executionContext);
            _userSecurityStampUpdateHelper.Update(user);

            using (var scope = _domainRepository.Transactions().CreateScope())
            {
                await _dbContext.SaveChangesAsync();
                await _userStoredProcedures.InvalidateUserAccountRecoveryRequests(user.UserId, executionContext.ExecutionDate);

                scope.QueueCompletionTask(() => OnTransactionComplete(user));
                await scope.CompleteAsync();
            }
        }

        private async Task OnTransactionComplete(User user)
        {
            _userContextCache.Clear(user.UserId);
            await _userSecurityStampUpdateHelper.OnTransactionCompleteAsync(user);

            await _messageAggregator.PublishAsync(new UserPasswordUpdatedMessage()
            {
                UserAreaCode = user.UserAreaCode,
                UserId = user.UserId
            });
        }

        private Task<User> GetUser(IExecutionContext executionContext)
        {
            return _dbContext
                .Users
                .FilterCanLogIn()
                .FilterById(executionContext.UserContext.UserId.Value)
                .SingleOrDefaultAsync();
        }

        private async Task ValidateMaxLoginAttemptsNotExceededAsync(User dbUser, IExecutionContext executionContext)
        {
            var query = new HasExceededMaxLoginAttemptsQuery()
            {
                UserAreaCode = dbUser.UserAreaCode,
                Username = dbUser.Username
            };

            var hasExceededMaxLoginAttempts = await _domainRepository
                .WithContext(executionContext)
                .ExecuteQueryAsync(query);

            if (hasExceededMaxLoginAttempts)
            {
                UserValidationErrors.Authentication.TooManyFailedAttempts.Throw();
            }
        }

        private async Task ValidatePasswordAsync(UpdateCurrentUserPasswordCommand command, User user, IExecutionContext executionContext)
        {
            var userArea = _userAreaRepository.GetRequiredByCode(user.UserAreaCode);
            _passwordUpdateCommandHelper.ValidateUserArea(userArea);

            var context = NewPasswordValidationContext.MapFromUser(user);
            context.CurrentPassword = command.OldPassword;
            context.Password = command.NewPassword;
            context.PropertyName = nameof(command.NewPassword);
            context.ExecutionContext = executionContext;

            await _newPasswordValidationService.ValidateAsync(context);
        }

        private async Task AuthenticateAsync(UpdateCurrentUserPasswordCommand command, User user)
        {
            if (_userAuthenticationHelper.VerifyPassword(user, command.OldPassword) == PasswordVerificationResult.Failed)
            {
                var logFailedAttemptCommand = new LogFailedLoginAttemptCommand(user.UserAreaCode, user.Username);
                await _domainRepository.ExecuteCommandAsync(logFailedAttemptCommand);

                UserValidationErrors.Authentication.InvalidPassword.Throw(nameof(command.OldPassword));
            }
        }

        public IEnumerable<IPermissionApplication> GetPermissions(UpdateCurrentUserPasswordCommand command)
        {
            yield return new CurrentUserUpdatePermission();
        }
    }
}