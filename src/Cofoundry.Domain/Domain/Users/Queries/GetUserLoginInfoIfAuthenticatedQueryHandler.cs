using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;

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
        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;

        public GetUserLoginInfoIfAuthenticatedQueryHandler(
            CofoundryDbContext dbContext,
            UserAuthenticationHelper userAuthenticationHelper,
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor
            )
        {
            _userAuthenticationHelper = userAuthenticationHelper;
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
        }

        public async Task<UserLoginInfoAuthenticationResult> ExecuteAsync(GetUserLoginInfoIfAuthenticatedQuery query, IExecutionContext executionContext)
        {
            var result = new UserLoginInfoAuthenticationResult();

            if (string.IsNullOrWhiteSpace(query.Username) || string.IsNullOrWhiteSpace(query.Password))
            {
                result.Error = UserLoginInfoAuthenticationError.InvalidCredentials;
                return result;
            }

            var hasExceededMaxLoginAttempts = await HasExceededMaxLoginAttemptsAsync(query, executionContext);

            if (hasExceededMaxLoginAttempts)
            {
                result.Error = UserLoginInfoAuthenticationError.TooManyFailedAttempts;
                return result;
            }

            var user = await GetUserAsync(query);
            result.User = MapResult(query, user);

            if (result.User == null)
            {
                var logFailedAttemptCommand = new LogFailedLoginAttemptCommand(query.UserAreaCode, query.Username);
                await _commandExecutor.ExecuteAsync(logFailedAttemptCommand);

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

            return result;
        }

        private UserLoginInfo MapResult(GetUserLoginInfoIfAuthenticatedQuery query, User user)
        {
            if (_userAuthenticationHelper.IsPasswordCorrect(user, query.Password))
            {
                var result = new UserLoginInfo()
                {
                    RequirePasswordChange = user.RequirePasswordChange,
                    UserAreaCode = user.UserAreaCode,
                    UserId = user.UserId,
                    IsEmailConfirmed = user.IsEmailConfirmed
                };

                return result;
            }

            return null;
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

        private Task<bool> HasExceededMaxLoginAttemptsAsync(GetUserLoginInfoIfAuthenticatedQuery inputQuery, IExecutionContext executionContext)
        {
            var hasExceededMaxLoginAttemptsQuery = new HasExceededMaxLoginAttemptsQuery()
            {
                UserAreaCode = inputQuery.UserAreaCode,
                Username = inputQuery.Username
            };

            return _queryExecutor.ExecuteAsync(hasExceededMaxLoginAttemptsQuery, executionContext);
        }
    }
}
