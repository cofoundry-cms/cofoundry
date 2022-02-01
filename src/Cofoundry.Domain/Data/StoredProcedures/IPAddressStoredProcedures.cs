using Cofoundry.Core.EntityFramework;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data.Internal
{
    /// <inheritdoc/>
    public class IPAddressStoredProcedures : IIPAddressStoredProcedures
    {
        private readonly IEntityFrameworkSqlExecutor _entityFrameworkSqlExecutor;
        private readonly CofoundryDbContext _dbContext;

        public IPAddressStoredProcedures(
            IEntityFrameworkSqlExecutor entityFrameworkSqlExecutor,
            CofoundryDbContext dbContext
            )
        {
            _entityFrameworkSqlExecutor = entityFrameworkSqlExecutor;
            _dbContext = dbContext;
        }

        public async Task<int> AddIfNotExistsAsync(
            string address,
            DateTime dateNow
            )
        {
            const string SP_NAME = "Cofoundry.IPAddress_AddIfNotExists";

            var ipAddressId = await _entityFrameworkSqlExecutor
                .ExecuteCommandWithOutputAsync<int?>(_dbContext,
                    SP_NAME,
                    "IPAddressId",
                     new SqlParameter("@Address", address),
                     new SqlParameter("@DateNow", dateNow)
                 );

            if (!ipAddressId.HasValue)
            {
                throw new UnexpectedStoredProcedureResultException(SP_NAME, "No IPAddressId was returned.");
            }

            return ipAddressId.Value;
        }
    }
}
