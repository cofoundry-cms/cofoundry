using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data.Internal
{
    /// <inheritdoc/>
    public class AuthorizedTaskStoredProcedures : IAuthorizedTaskStoredProcedures
    {
        private readonly IEntityFrameworkSqlExecutor _entityFrameworkSqlExecutor;
        private readonly CofoundryDbContext _dbContext;

        public AuthorizedTaskStoredProcedures(
            IEntityFrameworkSqlExecutor entityFrameworkSqlExecutor,
            CofoundryDbContext dbContext
            )
        {
            _entityFrameworkSqlExecutor = entityFrameworkSqlExecutor;
            _dbContext = dbContext;
        }

        public async Task InvalidateUserAccountRecoveryRequestsAsync(
            int userId,
            string[] authorizedTaskTypeCodes,
            DateTime dateNow
            )
        {
            string authorizedTaskTypeCodesParameterValue = null;
            
            if (!EnumerableHelper.IsNullOrEmpty(authorizedTaskTypeCodes))
            {
                authorizedTaskTypeCodesParameterValue = string.Join(",", authorizedTaskTypeCodes);
            }

            await _entityFrameworkSqlExecutor
                .ExecuteCommandAsync(_dbContext,
                    "Cofoundry.AuthorizedTask_Invalidate",
                     new SqlParameter("@userId", userId),
                     new SqlParameter("@AuthorizedTaskTypeCodes", authorizedTaskTypeCodesParameterValue),
                     new SqlParameter("@DateNow", dateNow)
                 );
        }

        public Task CleanupAsync(double retentionPeriodInSeconds, DateTime dateNow)
        {
            return _entityFrameworkSqlExecutor
                .ExecuteCommandAsync(_dbContext,
                "Cofoundry.AuthorizedTask_Cleanup",
                 new SqlParameter("@RetentionPeriodInSeconds", retentionPeriodInSeconds),
                 new SqlParameter("@DateNow", dateNow)
                 );
        }
    }
}