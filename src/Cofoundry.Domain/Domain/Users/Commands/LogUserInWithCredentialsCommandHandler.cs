using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;
using Microsoft.Extensions.Logging;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Logs a user into the application for a specified user area
    /// using username and password credentials. Checks for valid
    /// credentials and includes additional security checking such
    /// as preventing excessive login attempts. Validation errors
    /// are thrown as ValidationExceptions.
    /// </summary>
    public class LogUserInWithCredentialsCommandHandler
        : ICommandHandler<LogUserInWithCredentialsCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly ILogger<LogUserInWithCredentialsCommandHandler> _logger;
        private readonly CofoundryDbContext _dbContext;

        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly ILoginService _loginService;
        private readonly IPasswordUpdateCommandHelper _passwordUpdateCommandHelper;

        public LogUserInWithCredentialsCommandHandler(
            ILogger<LogUserInWithCredentialsCommandHandler> logger,
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            ILoginService loginService,
            IPasswordUpdateCommandHelper passwordUpdateCommandHelper
            )
        {
            _logger = logger;
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _loginService = loginService;
            _passwordUpdateCommandHelper = passwordUpdateCommandHelper;
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

            if (user.PasswordRehashNeeded)
            {
                await RehashPassword(user.UserId, command.Password);
            }

            if (user.RequirePasswordChange)
            {
                throw new PasswordChangeRequiredException();
            }

            ValidateLoginArea(command.UserAreaCode, user.UserAreaCode);
            await _loginService.LogAuthenticatedUserInAsync(command.UserAreaCode, user.UserId, command.RememberUser);
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

        /// <remarks>
        /// So far this is only used here, but it could be separated into it's own
        /// command if it was used elsewhere. 
        /// </remarks>
        private async Task RehashPassword(int userId, string password)
        {
            var user = await _dbContext
                .Users
                .SingleOrDefaultAsync(u => u.UserId == userId);

            EntityNotFoundException.ThrowIfNull(user, userId);

            _logger.LogDebug("Rehashing password for user {UserId}", user.UserId);
            _passwordUpdateCommandHelper.UpdatePasswordHash(password, user);

            await _dbContext.SaveChangesAsync();
        }
    }
}
