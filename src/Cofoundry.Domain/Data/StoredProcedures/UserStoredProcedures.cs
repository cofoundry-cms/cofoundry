using Cofoundry.Core.EntityFramework;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data.Internal
{
    /// <inheritdoc/>
    public class UserStoredProcedures : IUserStoredProcedures
    {
        private readonly IEntityFrameworkSqlExecutor _entityFrameworkSqlExecutor;
        private readonly CofoundryDbContext _dbContext;

        public UserStoredProcedures(
            IEntityFrameworkSqlExecutor entityFrameworkSqlExecutor,
            CofoundryDbContext dbContext
            )
        {
            _entityFrameworkSqlExecutor = entityFrameworkSqlExecutor;
            _dbContext = dbContext;
        }

        public async Task<int> AddEmailDomainIfNotExistsAsync(
            string name,
            string uniqueName,
            DateTime dateNow
            )
        {
            const string SP_NAME = "Cofoundry.EmailDomain_AddIfNotExists";

            var emailDomainId = await _entityFrameworkSqlExecutor
                .ExecuteCommandWithOutputAsync<int?>(_dbContext,
                SP_NAME,
                    "EmailDomainId",
                     new SqlParameter("Name", name),
                     new SqlParameter("UniqueName", uniqueName),
                     new SqlParameter("DateNow", dateNow)
                 );

            if (!emailDomainId.HasValue)
            {
                throw new UnexpectedStoredProcedureResultException(SP_NAME, "No EmailDomainId was returned.");
            }

            return emailDomainId.Value;
        }

        public async Task LogAuthenticationSuccess(int userId, string ipAddress, DateTime dateNow)
        {
            await _entityFrameworkSqlExecutor.ExecuteCommandAsync(_dbContext,
                   "Cofoundry.UserAuthenticationLog_Add",
                   new SqlParameter("UserId", userId),
                   new SqlParameter("IPAddress", ipAddress),
                   new SqlParameter("DateNow", dateNow)
                   );
        }

        public async Task LogAuthenticationFailed(string userAreaCode, string username, string ipAddress, DateTime dateNow)
        {
            await _entityFrameworkSqlExecutor.ExecuteCommandAsync(_dbContext,
                   "Cofoundry.UserAuthenticationFailLog_Add",
                   new SqlParameter("UserAreaCode", userAreaCode),
                   new SqlParameter("Username", username),
                   new SqlParameter("IPAddress", ipAddress),
                   new SqlParameter("DateNow", dateNow)
                   );
        }

        public async Task<bool> IsAuthenticationAttemptValid(
            string userAreaCode,
            string username, 
            string ipAddress, 
            DateTime dateNow, 
            int? ipAddressRateLimitQuantity, 
            int? ipAddressRateLimitWindowInSeconds, 
            int? usernameRateLimitQuantity, 
            int? usernameRateLimitWindowInSeconds
            )
        {
            var isValid = await _entityFrameworkSqlExecutor.ExecuteScalarAsync<int>(_dbContext,
                "Cofoundry.UserAuthenticationFailLog_IsAttemptValid",
                new SqlParameter("UserAreaCode", userAreaCode),
                new SqlParameter("Username", username),
                new SqlParameter("IPAddress", ipAddress),
                new SqlParameter("DateNow", dateNow),
                new SqlParameter("IPAddressRateLimitQuantity", ipAddressRateLimitQuantity),
                new SqlParameter("IPAddressRateLimitWindowInSeconds", ipAddressRateLimitWindowInSeconds),
                new SqlParameter("UsernameRateLimitQuantity", usernameRateLimitQuantity),
                new SqlParameter("UsernameRateLimitWindowInSeconds", usernameRateLimitWindowInSeconds)
                );

            return isValid == 1;
        }

        public async Task CleanupAsync(
            string userAreaCode,
            double authenticationLogRetentionPeriodInSeconds, 
            double authenticationFailedLogRetentionPeriodInSeconds,
            DateTime dateNow
            )
        {
            await _entityFrameworkSqlExecutor.ExecuteCommandAsync(_dbContext,
                   "Cofoundry.Users_Cleanup",
                   new SqlParameter("UserAreaCode", userAreaCode),
                   new SqlParameter("AuthenticationLogRetentionPeriodInSeconds", authenticationLogRetentionPeriodInSeconds),
                   new SqlParameter("AuthenticationFailedLogRetentionPeriodInSeconds", authenticationFailedLogRetentionPeriodInSeconds),
                   new SqlParameter("DateNow", dateNow)
                   );
        }
    }
}