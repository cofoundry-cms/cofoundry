using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Logs a user into the application for a specified user area
    /// using username and password credentials. Checks for valid
    /// credentials and includes additional security checking such
    /// as preventing excessive login attempts. Validation errors
    /// are thrown as ValidationExceptions.
    /// </summary>
    public class LogUserInWithCredentialsCommandHandler
        : IAsyncCommandHandler<LogUserInWithCredentialsCommand>
        , IIgnorePermissionCheckHandler
    {
        #region constructor
        
        private readonly IQueryExecutor _queryExecutor;
        private readonly ILoginService _loginService;
        private readonly ICommandExecutor _commandExecutor;

        public LogUserInWithCredentialsCommandHandler(
            IQueryExecutor queryExecutor,
            ILoginService loginService,
            ICommandExecutor commandExecutor
            )
        {
            _queryExecutor = queryExecutor;
            _loginService = loginService;
            _commandExecutor = commandExecutor;
        }

        #endregion

        public async Task ExecuteAsync(LogUserInWithCredentialsCommand command, IExecutionContext executionContext)
        {
            if (IsLoggedInAlready(command, executionContext)) return;

            var authResult = await GetUserLoginInfoAsync(command, executionContext);
            authResult.ThrowIfUnsuccessful(nameof(command.Password));

            if (authResult.User.RequirePasswordChange)
            {
                throw new PasswordChangeRequiredException();
            }

            ValidateLoginArea(command.UserAreaCode, authResult.User.UserAreaCode);
            await _loginService.LogAuthenticatedUserInAsync(
                command.UserAreaCode, 
                authResult.User.UserId, 
                command.RememberUser
                );
        }
        
        private static bool IsLoggedInAlready(LogUserInWithCredentialsCommand command, IExecutionContext executionContext)
        {
            var currentContext = executionContext.UserContext;
            var isLoggedIntoDifferentUserArea = currentContext.UserArea?.UserAreaCode != command.UserAreaCode;

            return currentContext.UserId.HasValue && !isLoggedIntoDifferentUserArea;
        }

        private Task<UserLoginInfoAuthenticationResult> GetUserLoginInfoAsync(LogUserInWithCredentialsCommand command, IExecutionContext executionContext)
        {
            var query = new GetUserLoginInfoIfAuthenticatedQuery()
            {
                UserAreaCode = command.UserAreaCode,
                Username = command.Username,
                Password = command.Password,
            };

            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        private static void ValidateLoginArea(string userAreaToLogInto, string usersUserArea)
        {
            if (userAreaToLogInto != usersUserArea)
            {
                throw new ValidationException("This user account is not permitted to log in via this route.");
            }
        }
    }
}
