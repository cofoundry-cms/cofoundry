using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
        
        private readonly IQueryExecutor _queryExecutor;
        private readonly ILoginService _loginService;

        public LogUserInWithCredentialsCommandHandler(
            IQueryExecutor queryExecutor,
            ILoginService loginService
            )
        {
            _queryExecutor = queryExecutor;
            _loginService = loginService;
        }

        #endregion

        public async Task ExecuteAsync(LogUserInWithCredentialsCommand command, IExecutionContext executionContext)
        {
            if (IsLoggedInAlready(command, executionContext)) return;

            var hasExceededMaxLoginAttempts = await _queryExecutor.ExecuteAsync(GetMaxLoginAttemptsQuery(command), executionContext);
            ValidateMaxLoginAttemptsNotExceeded(hasExceededMaxLoginAttempts);

            var user = await GetUserLoginInfoAsync(command, executionContext);

            if (user == null)
            {
                await _loginService.LogFailedLoginAttemptAsync(command.UserAreaCode, command.Username);

                throw new InvalidCredentialsAuthenticationException(nameof(command.Password));
            }

            if (user.RequirePasswordChange)
            {
                throw new PasswordChangeRequiredException();
            }

            ValidateLoginArea(command.UserAreaCode, user.UserAreaCode);
            await _loginService.LogAuthenticatedUserInAsync(command.UserAreaCode, user.UserId, true);
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
                throw new TooManyFailedAttemptsAuthenticationException();
            }
        }

        private Task<UserLoginInfo> GetUserLoginInfoAsync(LogUserInWithCredentialsCommand command, IExecutionContext executionContext)
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
