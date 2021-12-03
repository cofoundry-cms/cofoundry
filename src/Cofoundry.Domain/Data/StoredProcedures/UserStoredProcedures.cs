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

        public async Task<int> AddEmailDomainIfNotExists(
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
                     new SqlParameter("@Name", name),
                     new SqlParameter("@UniqueName", uniqueName),
                     new SqlParameter("@DateNow", dateNow)
                 );

            if (!emailDomainId.HasValue)
            {
                throw new UnexpectedStoredProcedureResultException(SP_NAME, "No EmailDomainId was returned.");
            }

            return emailDomainId.Value;
        }
    }
}
