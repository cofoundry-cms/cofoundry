using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core.Validation;
using Cofoundry.Core;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Updates the password of the currently logged in user, using the
    /// OldPassword field to authenticate the request.
    /// </summary>
    public class UpdateCurrentUserPasswordCommandHandler 
        : IAsyncCommandHandler<UpdateCurrentUserPasswordCommand>
        , IPermissionRestrictedCommandHandler<UpdateCurrentUserPasswordCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;
        private readonly UserAuthenticationHelper _userAuthenticationHelper;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly IUserAreaDefinitionRepository _userAreaRepository;
        private readonly IPasswordUpdateCommandHelper _passwordUpdateCommandHelper;

        public UpdateCurrentUserPasswordCommandHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor,
            UserAuthenticationHelper userAuthenticationHelper,
            IPermissionValidationService permissionValidationService,
            IUserAreaDefinitionRepository userAreaRepository,
            IPasswordUpdateCommandHelper passwordUpdateCommandHelper
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
            _userAuthenticationHelper = userAuthenticationHelper;
            _permissionValidationService = permissionValidationService;
            _userAreaRepository = userAreaRepository;
            _passwordUpdateCommandHelper = passwordUpdateCommandHelper;
        }

        #endregion

        #region execution
        
        public async Task ExecuteAsync(UpdateCurrentUserPasswordCommand command, IExecutionContext executionContext)
        {
            _permissionValidationService.EnforceIsLoggedIn(executionContext.UserContext);

            var user = await GetUser(command, executionContext);

            await ValidateMaxLoginAttemptsNotExceededAsync(user, executionContext);

            await UpdatePasswordAsync(command, executionContext, user);
        }

        private async Task UpdatePasswordAsync(UpdateCurrentUserPasswordCommand command, IExecutionContext executionContext, User user)
        {
            EntityNotFoundException.ThrowIfNull(user, executionContext.UserContext.UserId);
            var userArea = _userAreaRepository.GetByCode(user.UserAreaCode);

            _passwordUpdateCommandHelper.ValidateUserArea(userArea);

            if (!_userAuthenticationHelper.IsPasswordCorrect(user, command.OldPassword))
            {
                var logFailedAttemptCommand = new LogFailedLoginAttemptCommand(user.UserAreaCode, user.Username);
                await _commandExecutor.ExecuteAsync(logFailedAttemptCommand);

                throw new InvalidCredentialsAuthenticationException(nameof(command.OldPassword), "Incorrect password");
            }

            _passwordUpdateCommandHelper.UpdatePassword(command.NewPassword, user, executionContext);
            await _dbContext.SaveChangesAsync();
        }

        private Task<User> GetUser(UpdateCurrentUserPasswordCommand command, IExecutionContext executionContext)
        {
            return _dbContext
                .Users
                .FilterCanLogIn()
                .FilterById(executionContext.UserContext.UserId.Value)
                .SingleOrDefaultAsync();
        }

        private async Task ValidateMaxLoginAttemptsNotExceededAsync(User user, IExecutionContext executionContext)
        {
            var query = new HasExceededMaxLoginAttemptsQuery()
            {
                UserAreaCode = user.UserAreaCode,
                Username = user.Username
            };

            var hasExceededMaxLoginAttempts = await _queryExecutor.ExecuteAsync(query, executionContext);

            if (hasExceededMaxLoginAttempts)
            {
                throw new TooManyFailedAttemptsAuthenticationException();
            }
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(UpdateCurrentUserPasswordCommand command)
        {
            yield return new CurrentUserUpdatePermission();
        }

        #endregion
    }
}
