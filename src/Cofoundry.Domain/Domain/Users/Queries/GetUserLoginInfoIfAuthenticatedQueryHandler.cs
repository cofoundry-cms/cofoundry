using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// A query handler that gets information about a user if the specified credentials
    /// pass an authentication check
    /// </summary>
    public class GetUserLoginInfoIfAuthenticatedQueryHandler
        : IQueryHandler<GetUserLoginInfoIfAuthenticatedQuery, UserLoginInfoAuthenticationResult>
        , IIgnorePermissionCheckHandler
    {
        private readonly UserAuthenticationHelper _userAuthenticationHelper;
        private readonly ILogger<GetUserLoginInfoIfAuthenticatedQueryHandler> _logger;
        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;

        public GetUserLoginInfoIfAuthenticatedQueryHandler(
            ILogger<GetUserLoginInfoIfAuthenticatedQueryHandler> logger,
            CofoundryDbContext dbContext,
            UserAuthenticationHelper userAuthenticationHelper,
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor
            )
        {
            _userAuthenticationHelper = userAuthenticationHelper;
            _logger = logger;
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
        }

        public async Task<UserLoginInfoAuthenticationResult> ExecuteAsync(GetUserLoginInfoIfAuthenticatedQuery query, IExecutionContext executionContext)
        {
            if (string.IsNullOrWhiteSpace(query.Username) || string.IsNullOrWhiteSpace(query.Password))
            {
                return GetAuthenticationFailedForUnknownUserResult(query);
            }

            var hasExceededMaxLoginAttempts = await HasExceededMaxLoginAttemptsAsync(query, executionContext);
            if (hasExceededMaxLoginAttempts)
            {
                _logger.LogInformation("Authentication failed due to too many failed attempts {UserAreaCode}", query.UserAreaCode);
                return new UserLoginInfoAuthenticationResult()
                {
                    Error = UserLoginInfoAuthenticationError.TooManyFailedAttempts
                };
            }

            var dbUser = await GetUserAsync(query);
            if (dbUser == null)
            {
                await LogFailedLogginAttemptAsync(query);
                return GetAuthenticationFailedForUnknownUserResult(query);
            }

            var result = new UserLoginInfoAuthenticationResult();
            result.User = MapUser(query, dbUser);

            await FinalizeResultAsync(query, result);

            return result;
        }

        private UserLoginInfoAuthenticationResult GetAuthenticationFailedForUnknownUserResult(GetUserLoginInfoIfAuthenticatedQuery query)
        {
            _logger.LogInformation("Authentication failed for unknown user in user area {UserAreaCode}", query.UserAreaCode);

            var result = new UserLoginInfoAuthenticationResult();
            result.Error = UserLoginInfoAuthenticationError.InvalidCredentials;

            return result;
        }

        private Task<bool> HasExceededMaxLoginAttemptsAsync(GetUserLoginInfoIfAuthenticatedQuery inputQuery, IExecutionContext executionContext)
        {
            var hasExceededMaxLoginAttemptsQuery = new HasExceededMaxLoginAttemptsQuery()
            {
                UserAreaCode = inputQuery.UserAreaCode,
                Username = inputQuery.Username
            };

            return _queryExecutor.ExecuteAsync(hasExceededMaxLoginAttemptsQuery, executionContext);
        }

        private Task<User> GetUserAsync(GetUserLoginInfoIfAuthenticatedQuery query)
        {
            return _dbContext
                .Users
                .AsNoTracking()
                .FilterByUserArea(query.UserAreaCode)
                .FilterCanLogIn()
                .Where(u => u.Username == query.Username)
                .FirstOrDefaultAsync();
        }

        private async Task LogFailedLogginAttemptAsync(GetUserLoginInfoIfAuthenticatedQuery query)
        {
            await _commandExecutor.ExecuteAsync(new LogFailedLoginAttemptCommand(
                query.UserAreaCode,
                query.Username
                ));
        }

        private UserLoginInfo MapUser(GetUserLoginInfoIfAuthenticatedQuery query, User dbUser)
        {
            if (dbUser == null) throw new ArgumentNullException(nameof(dbUser));

            var verificationResult = VerifyPassword(query, dbUser);
            if (verificationResult == PasswordVerificationResult.Failed) return null;

            var userLoginInfo = new UserLoginInfo()
            {
                RequirePasswordChange = dbUser.RequirePasswordChange,
                UserAreaCode = dbUser.UserAreaCode,
                UserId = dbUser.UserId,
                IsEmailConfirmed = dbUser.IsEmailConfirmed,
                PasswordRehashNeeded = verificationResult == PasswordVerificationResult.SuccessRehashNeeded
            };

            return userLoginInfo;
        }

        private PasswordVerificationResult VerifyPassword(GetUserLoginInfoIfAuthenticatedQuery query, User dbUser)
        {
            if (dbUser == null) throw new ArgumentNullException(nameof(dbUser));
            var verificationResult = _userAuthenticationHelper.VerifyPassword(dbUser, query.Password);

            switch (verificationResult)
            {
                case PasswordVerificationResult.Failed:
                    _logger.LogInformation("Authentication failed for user {UserId}", dbUser.UserId);
                    break;
                case PasswordVerificationResult.SuccessRehashNeeded:
                    _logger.LogInformation("Authentication success for user {UserId} (rehash needed)", dbUser.UserId);
                    break;
                case PasswordVerificationResult.Success:
                    _logger.LogInformation("Authentication success for user {UserId}", dbUser.UserId);
                    break;
                default:
                    throw new InvalidOperationException("Unrecognised PasswordVerificationResult: " + verificationResult);
            }

            return verificationResult;
        }

        /// <summary>
        /// The user has been mapped, so complete the mapping of the outer result and handle
        /// any side-effects
        /// </summary>
        private async Task FinalizeResultAsync(GetUserLoginInfoIfAuthenticatedQuery query, UserLoginInfoAuthenticationResult result)
        {
            if (result.User == null)
            {
                await LogFailedLogginAttemptAsync(query);

                result.Error = UserLoginInfoAuthenticationError.InvalidCredentials;
            }
            else
            {
                if (result.Error != UserLoginInfoAuthenticationError.None)
                {
                    throw new InvalidOperationException($"Unexpected {nameof(UserLoginInfoAuthenticationError)} state for a successful request: {result.Error}");
                }
                result.IsSuccess = true;
            }
        }
    }
}
