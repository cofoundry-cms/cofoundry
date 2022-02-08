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
    /// Validates user credentials. If the authentication was successful then user information 
    /// pertinent to sign in is returned, otherwise error information is returned detailing
    /// why the authentication failed.
    /// </summary>
    public class ValidateUserCredentialsQueryHandler
        : IQueryHandler<ValidateUserCredentialsQuery, UserCredentialsValidationResult>
        , IIgnorePermissionCheckHandler
    {
        private readonly UserAuthenticationHelper _userAuthenticationHelper;
        private readonly ILogger<ValidateUserCredentialsQueryHandler> _logger;
        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IUserDataFormatter _userDataFormatter;

        public ValidateUserCredentialsQueryHandler(
            ILogger<ValidateUserCredentialsQueryHandler> logger,
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

        public async Task<UserCredentialsValidationResult> ExecuteAsync(ValidateUserCredentialsQuery query, IExecutionContext executionContext)
        {
            var uniqueUsername = _userDataFormatter.UniquifyUsername(query.UserAreaCode, query.Username);
            if (string.IsNullOrWhiteSpace(uniqueUsername) || string.IsNullOrWhiteSpace(query.Password))
            {
                return GetAuthenticationFailedForUnknownUserResult(query);
            }

            var hasExceededMaxAuthenticationInAttempts = await HasExceededMaxAuthenticationAttemptsAsync(query.UserAreaCode, uniqueUsername, executionContext);
            if (hasExceededMaxAuthenticationInAttempts)
            {
                _logger.LogDebug("Authentication failed due to too many failed attempts {UserAreaCode}", query.UserAreaCode);
                return new UserCredentialsValidationResult()
                {
                    Error = UserValidationErrors.Authentication.TooManyFailedAttempts.Create(query.PropertyToValidate)
                };
            }

            var dbUser = await GetUserAsync(query.UserAreaCode, uniqueUsername);
            if (dbUser == null)
            {
                await LogFailedAuthenticationAttemptAsync(query.UserAreaCode, uniqueUsername, executionContext);
                return GetAuthenticationFailedForUnknownUserResult(query);
            }

            var result = new UserCredentialsValidationResult();
            result.User = MapUser(query, dbUser);

            await FinalizeResultAsync(result, query, uniqueUsername, executionContext);

            return result;
        }

        private UserCredentialsValidationResult GetAuthenticationFailedForUnknownUserResult(ValidateUserCredentialsQuery query)
        {
            _logger.LogDebug("Authentication failed for unknown user in user area {UserAreaCode}", query.UserAreaCode);

            var result = new UserCredentialsValidationResult();
            result.Error = UserValidationErrors.Authentication.InvalidCredentials.Create(query.PropertyToValidate);

            return result;
        }

        private Task<bool> HasExceededMaxAuthenticationAttemptsAsync(string userAreaCode, string uniqueUsername, IExecutionContext executionContext)
        {
            var hasExceededMaxAuthenticationAttemptsQuery = new HasExceededMaxAuthenticationAttemptsQuery()
            {
                UserAreaCode = userAreaCode,
                Username = uniqueUsername
            };

            return _queryExecutor.ExecuteAsync(hasExceededMaxAuthenticationAttemptsQuery, executionContext);
        }

        private Task<User> GetUserAsync(string userAreaCode, string uniqueUsername)
        {
            return _dbContext
                .Users
                .AsNoTracking()
                .FilterByUserArea(userAreaCode)
                .FilterCanSignIn()
                .Where(u => u.UniqueUsername == uniqueUsername)
                .FirstOrDefaultAsync();
        }

        private async Task LogFailedAuthenticationAttemptAsync(string userAreaCode, string uniqueUsername, IExecutionContext executionContext)
        {
            var command = new LogFailedAuthenticationAttemptCommand(userAreaCode, uniqueUsername);
            await _commandExecutor.ExecuteAsync(command, executionContext);
        }

        private UserSignInInfo MapUser(ValidateUserCredentialsQuery query, User dbUser)
        {
            if (dbUser == null) throw new ArgumentNullException(nameof(dbUser));

            var verificationResult = VerifyPassword(query, dbUser);
            if (verificationResult == PasswordVerificationResult.Failed) return null;

            var userSignInInfo = new UserSignInInfo()
            {
                RequirePasswordChange = dbUser.RequirePasswordChange,
                UserAreaCode = dbUser.UserAreaCode,
                UserId = dbUser.UserId,
                IsAccountVerified = dbUser.AccountVerifiedDate.HasValue,
                PasswordRehashNeeded = verificationResult == PasswordVerificationResult.SuccessRehashNeeded
            };

            return userSignInInfo;
        }

        private PasswordVerificationResult VerifyPassword(ValidateUserCredentialsQuery query, User dbUser)
        {
            if (dbUser == null) throw new ArgumentNullException(nameof(dbUser));
            var verificationResult = _userAuthenticationHelper.VerifyPassword(dbUser, query.Password);

            switch (verificationResult)
            {
                case PasswordVerificationResult.Failed:
                    _logger.LogDebug("Authentication failed for user {UserId}", dbUser.UserId);
                    break;
                case PasswordVerificationResult.SuccessRehashNeeded:
                    _logger.LogDebug("Authentication success for user {UserId} (rehash needed)", dbUser.UserId);
                    break;
                case PasswordVerificationResult.Success:
                    _logger.LogDebug("Authentication success for user {UserId}", dbUser.UserId);
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
            UserCredentialsValidationResult result,
            ValidateUserCredentialsQuery query,
            string uniqueUsername,
            IExecutionContext executionContext
            )
        {
            if (result.User == null)
            {
                await LogFailedAuthenticationAttemptAsync(query.UserAreaCode, uniqueUsername, executionContext);

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