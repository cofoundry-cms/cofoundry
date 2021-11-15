using Cofoundry.Domain.CQS;
using System;
using System.Threading.Tasks;

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
        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IExecutionContextFactory _executionContextFactory;

        public UpdateUnauthenticatedUserPasswordCommandHandler(
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor,
            IExecutionContextFactory executionContextFactory
            )
        {
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
            _executionContextFactory = executionContextFactory;
        }

        public async Task ExecuteAsync(UpdateUnauthenticatedUserPasswordCommand command, IExecutionContext executionContext)
        {
            if (IsLoggedInAlready(command, executionContext))
            {
                throw new Exception("UpdateUnauthenticatedUserPasswordCommand cannot be used when the user is already logged in.");
            }

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
    }
}
