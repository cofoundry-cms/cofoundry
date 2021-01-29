using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Updates the password of the currently logged in user, using the
    /// OldPassword field to authenticate the request.
    /// </summary>
    public class UpdateUnauthenticatedUserPasswordCommandHandler
        : ICommandHandler<UpdateUnauthenticatedUserPasswordCommand>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly IUserAreaDefinitionRepository _userAreaRepository;
        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IExecutionContextFactory _executionContextFactory;

        public UpdateUnauthenticatedUserPasswordCommandHandler(
            IUserAreaDefinitionRepository userAreaRepository,
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor,
            IExecutionContextFactory executionContextFactory
            )
        {
            _userAreaRepository = userAreaRepository;
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
            _executionContextFactory = executionContextFactory;
        }

        #endregion

        public async Task ExecuteAsync(UpdateUnauthenticatedUserPasswordCommand command, IExecutionContext executionContext)
        {
            if (IsLoggedInAlready(command, executionContext))
            {
                throw new Exception("UpdateUnauthenticatedUserPasswordCommand cannot be used when the user is already logged in.");
            }

            var userArea = _userAreaRepository.GetByCode(command.UserAreaCode);

            var authResult = await GetUserLoginInfoAsync(command, executionContext);
            authResult.ThrowIfUnsuccessful(nameof(command.OldPassword));

            var updatePasswordCommand = new UpdateUserPasswordByUserIdCommand()
            {
                UserId = authResult.User.UserId,
                NewPassword = command.NewPassword
            };

            // User is not logged in, so will need to elevate permissions here to change the password.
            var systemExecutionContext = await _executionContextFactory.CreateSystemUserExecutionContextAsync(executionContext);
            await _commandExecutor.ExecuteAsync(updatePasswordCommand, systemExecutionContext);

            // We pass out the userid since we do the auth inside the command and it might be useful to the callee
            command.OutputUserId = authResult.User.UserId;
        }

        private Task<UserLoginInfoAuthenticationResult> GetUserLoginInfoAsync(
            UpdateUnauthenticatedUserPasswordCommand command, 
            IExecutionContext executionContext
            )
        {
            var query = new GetUserLoginInfoIfAuthenticatedQuery()
            {
                UserAreaCode = command.UserAreaCode,
                Username = command.Username,
                Password = command.OldPassword,
            };

            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        private static bool IsLoggedInAlready(UpdateUnauthenticatedUserPasswordCommand command, IExecutionContext executionContext)
        {
            var currentContext = executionContext.UserContext;
            var isLoggedIntoDifferentUserArea = currentContext.UserArea?.UserAreaCode != command.UserAreaCode;

            return currentContext.UserId.HasValue && !isLoggedIntoDifferentUserArea;
        }

        private async Task ValidateMaxLoginAttemptsNotExceeded(UpdateUnauthenticatedUserPasswordCommand command, IExecutionContext executionContext)
        {
            var query = new HasExceededMaxLoginAttemptsQuery()
            {
                UserAreaCode = command.UserAreaCode,
                Username = command.Username
            };

            var hasAttemptsExceeded = await _queryExecutor.ExecuteAsync(query, executionContext);
            if (hasAttemptsExceeded)
            {
                throw new TooManyFailedAttemptsAuthenticationException();
            }
        }
    }
}
