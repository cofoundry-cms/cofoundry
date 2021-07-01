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
        : IQueryHandler<GetUserLoginInfoIfAuthenticatedQuery, UserLoginInfo>
        , IIgnorePermissionCheckHandler
    {
        private readonly UserAuthenticationHelper _userAuthenticationHelper;
        private readonly ILogger<GetUserLoginInfoIfAuthenticatedQueryHandler> _logger;
        private readonly CofoundryDbContext _dbContext;

        public GetUserLoginInfoIfAuthenticatedQueryHandler(
            ILogger<GetUserLoginInfoIfAuthenticatedQueryHandler> logger,
            CofoundryDbContext dbContext,
            UserAuthenticationHelper userAuthenticationHelper
            )
        {
            _userAuthenticationHelper = userAuthenticationHelper;
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<UserLoginInfo> ExecuteAsync(GetUserLoginInfoIfAuthenticatedQuery query, IExecutionContext executionContext)
        {
            if (string.IsNullOrWhiteSpace(query.Username) || string.IsNullOrWhiteSpace(query.Password)) return null;

            var dbUser = await GetUserAsync(query);

            _logger.LogInformation("Authentication failed for unknown user in user area {UserAreaCode}", query.UserAreaCode);
            if (dbUser == null) return null;

            var verificationResult = _userAuthenticationHelper.VerifyPassword(dbUser, query.Password);

            switch (verificationResult)
            {
                case PasswordVerificationResult.Failed:
                    _logger.LogInformation("Authentication failed for user {UserId}", dbUser.UserId);
                    return null;
                case PasswordVerificationResult.SuccessRehashNeeded:
                    _logger.LogInformation("Authentication success for user {UserId} (rehash needed)", dbUser.UserId);
                    break;
                case PasswordVerificationResult.Success:
                    _logger.LogInformation("Authentication success for user {UserId}", dbUser.UserId);
                    break;
                default:
                    throw new InvalidOperationException("Unrecognised PasswordVerificationResult: " + verificationResult);
            }

            var result = MapResult(dbUser, verificationResult);

            return result;
        }

        private UserLoginInfo MapResult(User user, PasswordVerificationResult verificationResult)
        {
            var result = new UserLoginInfo()
            {
                RequirePasswordChange = user.RequirePasswordChange,
                UserAreaCode = user.UserAreaCode,
                UserId = user.UserId,
                PasswordRehashNeeded = verificationResult == PasswordVerificationResult.SuccessRehashNeeded
            };

            return result;
        }

        private Task<User> GetUserAsync(GetUserLoginInfoIfAuthenticatedQuery query)
        {
            return _dbContext
                .Users
                .FilterByUserArea(query.UserAreaCode)
                .FilterCanLogIn()
                .Where(u => u.Username == query.Username)
                .FirstOrDefaultAsync();
        }
    }
}
