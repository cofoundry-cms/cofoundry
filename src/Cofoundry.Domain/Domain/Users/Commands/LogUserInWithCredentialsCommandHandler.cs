using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Entity;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.EntityFramework;
using Cofoundry.Core;
using System.ComponentModel.DataAnnotations;
using Cofoundry.Core.Validation;

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

        private readonly ICommandExecutor _commandExecutor;
        private readonly IQueryExecutor _queryExecutor;
        private readonly ILoginService _loginService;

        public LogUserInWithCredentialsCommandHandler(
            ICommandExecutor commandExecutor,
            IQueryExecutor queryExecutor,
            ILoginService loginService
            )
        {
            _commandExecutor = commandExecutor;
            _queryExecutor = queryExecutor;
            _loginService = loginService;
        }

        #endregion

        public async Task ExecuteAsync(LogUserInWithCredentialsCommand command, IExecutionContext executionContext)
        {
            if (IsLoggedInAlready(command, executionContext)) return;

            var hasExceededMaxLoginAttempts = await _queryExecutor.ExecuteAsync(GetMaxLoginAttemptsQuery(command));
            ValidateMaxLoginAttemptsNotExceeded(hasExceededMaxLoginAttempts);

            var user = await _queryExecutor.ExecuteAsync(GetAuthenticationQuery(command));

            if (user == null)
            {
                await _loginService.LogFailedLoginAttemptAsync(command.UserAreaCode, command.Username);

                ThrowInvalidCredentialsError(command);
            }

            ValidateLoginArea(command.UserAreaCode, user.UserAreaCode);
            await _loginService.LogAuthenticatedUserInAsync(user.UserId, true);
        }
        
        private static bool IsLoggedInAlready(LogUserInWithCredentialsCommand command, IExecutionContext executionContext)
        {
            var currentContext = executionContext.UserContext;
            var isLoggedIntoDifferentUserArea = currentContext.UserArea?.UserAreaCode != command.UserAreaCode;

            return currentContext.UserId.HasValue && !isLoggedIntoDifferentUserArea;
        }

        private static HasExceededMaxLoginAttemptsQuery GetMaxLoginAttemptsQuery(LogUserInWithCredentialsCommand command)
        {
            return new HasExceededMaxLoginAttemptsQuery()
            {
                UserAreaCode = command.UserAreaCode,
                Username = command.Username
            };
        }

        private static void ValidateMaxLoginAttemptsNotExceeded(bool hasAttemptsExceeded)
        {
            if (hasAttemptsExceeded)
            {
                throw new ValidationException("Too many failed login attempts have been detected, please try again later.");
            }
        }

        private static GetUserLoginInfoIfAuthenticatedQuery GetAuthenticationQuery(LogUserInWithCredentialsCommand command)
        {
            return new GetUserLoginInfoIfAuthenticatedQuery()
            {
                Username = command.Username,
                Password = command.Password
            };
        }

        private static void ThrowInvalidCredentialsError(LogUserInWithCredentialsCommand command)
        {
            throw new PropertyValidationException("Invalid username or password", nameof(command.Password));
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
