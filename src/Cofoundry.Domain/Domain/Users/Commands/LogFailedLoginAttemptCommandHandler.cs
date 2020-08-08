using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.EntityFramework;
using Microsoft.Data.SqlClient;
using Cofoundry.Core;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal
{
    public class LogFailedLoginAttemptCommandHandler
        : ICommandHandler<LogFailedLoginAttemptCommand>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IEntityFrameworkSqlExecutor _sqlExecutor;
        private readonly IClientConnectionService _clientConnectionService;

        public LogFailedLoginAttemptCommandHandler(
            CofoundryDbContext dbContext,
            IEntityFrameworkSqlExecutor sqlExecutor,
            IClientConnectionService clientConnectionService
            )
        {
            _dbContext = dbContext;
            _sqlExecutor = sqlExecutor;
            _clientConnectionService = clientConnectionService;
        }
        #endregion

        public async Task ExecuteAsync(LogFailedLoginAttemptCommand command, IExecutionContext executionContext)
        {
            var connectionInfo = _clientConnectionService.GetConnectionInfo();

            await _sqlExecutor.ExecuteCommandAsync(_dbContext,
                "Cofoundry.FailedAuthticationAttempt_Add",
                new SqlParameter("UserAreaCode", command.UserAreaCode),
                new SqlParameter("Username", TextFormatter.Limit(command.Username, 150)),
                new SqlParameter("IPAddress", connectionInfo.IPAddress),
                new SqlParameter("DateTimeNow", executionContext.ExecutionDate)
                );
        }
    }
}
