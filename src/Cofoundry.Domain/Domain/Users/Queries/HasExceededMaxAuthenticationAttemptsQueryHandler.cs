using Cofoundry.Core.EntityFramework;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class HasExceededMaxAuthenticationAttemptsQueryHandler
        : IQueryHandler<HasExceededMaxAuthenticationAttemptsQuery, bool>
        , IIgnorePermissionCheckHandler
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IEntityFrameworkSqlExecutor _sqlExecutor;
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly IClientConnectionService _clientConnectionService;

        public HasExceededMaxAuthenticationAttemptsQueryHandler(
            CofoundryDbContext dbContext,
            IEntityFrameworkSqlExecutor sqlExecutor,
            AuthenticationSettings authenticationSettings,
            IClientConnectionService clientConnectionService
            )
        {
            _dbContext = dbContext;
            _sqlExecutor = sqlExecutor;
            _authenticationSettings = authenticationSettings;
            _clientConnectionService = clientConnectionService;
        }

        public async Task<bool> ExecuteAsync(HasExceededMaxAuthenticationAttemptsQuery query, IExecutionContext executionContext)
        {
            var connectionInfo = _clientConnectionService.GetConnectionInfo();

            var isValid = await _sqlExecutor.ExecuteScalarAsync<int>(_dbContext,
                "Cofoundry.FailedAuthticationAttempt_IsAttemptValid",
                new SqlParameter("UserAreaCode", query.UserAreaCode),
                new SqlParameter("Username", query.Username),
                new SqlParameter("IPAddress", connectionInfo.IPAddress),
                new SqlParameter("DateTimeNow", executionContext.ExecutionDate),
                new SqlParameter("MaxIPAttempts", _authenticationSettings.MaxIPAttempts),
                new SqlParameter("MaxUsernameAttempts", _authenticationSettings.MaxUsernameAttempts),
                new SqlParameter("MaxIPAttemptsBoundaryInMinutes", _authenticationSettings.MaxIPAttemptsBoundaryInMinutes),
                new SqlParameter("MaxUsernameAttemptsBoundaryInMinutes", _authenticationSettings.MaxUsernameAttemptsBoundaryInMinutes)
                );

            return isValid != 1;
        }
    }

}
