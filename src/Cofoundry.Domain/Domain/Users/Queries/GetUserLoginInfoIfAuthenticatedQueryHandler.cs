using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Returns information about a user if the specified credentials
    /// pass an authentication check.
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
        private readonly IUserDataFormatter _userDataFormatter;

        public GetUserLoginInfoIfAuthenticatedQueryHandler(
            ILogger<GetUserLoginInfoIfAuthenticatedQueryHandler> logger,
            CofoundryDbContext dbContext,
            UserAuthenticationHelper userAuthenticationHelper,
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor,
            IUserDataFormatter userDataFormatter
            )
        {
            _userAuthenticationHelper = userAuthenticationHelper;
            _logger = logger;
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
            _userDataFormatter = userDataFormatter;
        }

        public async Task<UserLoginInfoAuthenticationResult> ExecuteAsync(GetUserLoginInfoIfAuthenticatedQuery query, IExecutionContext executionContext)
        {
            var uniqueUsername = _userDataFormatter.UniquifyUsername(query.UserAreaCode, query.Username);
            if (string.IsNullOrWhiteSpace(uniqueUsername) || string.IsNullOrWhiteSpace(query.Password))
            {
                return GetAuthenticationFailedForUnknownUserResult(query);
            }

            var hasExceededMaxLoginAttempts = await HasExceededMaxLoginAttemptsAsync(query.UserAreaCode, uniqueUsername, executionContext);
            if (hasExceededMaxLoginAttempts)
            {
                _logger.LogInformation("Authentication failed due to too many failed attempts {UserAreaCode}", query.UserAreaCode);
                return new UserLoginInfoAuthenticationResult()
                {
                    Error = UserValidationErrors.Authentication.TooManyFailedAttempts.Create(query.PropertyToValidate)
                };
            }

            var dbUser = await GetUserAsync(query.UserAreaCode, uniqueUsername);
            if (dbUser == null)
            {
                await LogFailedLogginAttemptAsync(query.UserAreaCode, uniqueUsername, executionContext);
                return GetAuthenticationFailedForUnknownUserResult(query);
            }

            var result = new UserLoginInfoAuthenticationResult();
            result.User = MapUser(query, dbUser);

            await FinalizeResultAsync(result, query, uniqueUsername, executionContext);

            return result;
        }

        private UserLoginInfoAuthenticationResult GetAuthenticationFailedForUnknownUserResult(GetUserLoginInfoIfAuthenticatedQuery query)
        {
            _logger.LogInformation("Authentication failed for unknown user in user area {UserAreaCode}", query.UserAreaCode);

            var result = new UserLoginInfoAuthenticationResult();
            result.Error = UserValidationErrors.Authentication.InvalidCredentials.Create(query.PropertyToValidate);

            return result;
        }

        private Task<bool> HasExceededMaxLoginAttemptsAsync(string userAreaCode, string uniqueUsername, IExecutionContext executionContext)
        {
            var hasExceededMaxLoginAttemptsQuery = new HasExceededMaxLoginAttemptsQuery()
            {
                UserAreaCode = userAreaCode,
                Username = uniqueUsername
            };

            return _queryExecutor.ExecuteAsync(hasExceededMaxLoginAttemptsQuery, executionContext);
        }

        private Task<User> GetUserAsync(string userAreaCode, string uniqueUsername)
        {
            return _dbContext
                .Users
                .AsNoTracking()
                .FilterByUserArea(userAreaCode)
                .FilterCanLogIn()
                .Where(u => u.UniqueUsername == uniqueUsername)
                .FirstOrDefaultAsync();
        }

        private async Task LogFailedLogginAttemptAsync(string userAreaCode, string uniqueUsername, IExecutionContext executionContext)
        {
            var command = new LogFailedLoginAttemptCommand(userAreaCode, uniqueUsername);
            await _commandExecutor.ExecuteAsync(command, executionContext);
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
        private async Task FinalizeResultAsync(
            UserLoginInfoAuthenticationResult result,
            GetUserLoginInfoIfAuthenticatedQuery query,
            string uniqueUsername,
            IExecutionContext executionContext
            )
        {
            if (result.User == null)
            {
                await LogFailedLogginAttemptAsync(query.UserAreaCode, uniqueUsername, executionContext);

                result.Error = UserValidationErrors.Authentication.InvalidCredentials.Create(query.PropertyToValidate);
            }
            else
            {
                if (result.Error != null)
                {
                    throw new InvalidOperationException($"Unexpected error state for a successful request: {result.Error.ErrorCode}");
                }
                result.IsSuccess = true;
            }
        }
    }
}
