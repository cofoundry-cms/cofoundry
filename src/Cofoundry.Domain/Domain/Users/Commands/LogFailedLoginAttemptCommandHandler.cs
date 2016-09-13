using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.EntityFramework;
using System.Data.SqlClient;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class LogFailedLoginAttemptCommandHandler
        : ICommandHandler<LogFailedLoginAttemptCommand>
        , IAsyncCommandHandler<LogFailedLoginAttemptCommand>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly IEntityFrameworkSqlExecutor _sqlExecutor;
        private readonly IClientConnectionService _clientConnectionService;

        public LogFailedLoginAttemptCommandHandler(
            IEntityFrameworkSqlExecutor sqlExecutor,
            IClientConnectionService clientConnectionService
            )
        {
            _sqlExecutor = sqlExecutor;
            _clientConnectionService = clientConnectionService;
        }
        #endregion

        public void Execute(LogFailedLoginAttemptCommand command, IExecutionContext executionContext)
        {
            var connectionInfo = _clientConnectionService.GetConnectionInfo();

            _sqlExecutor.ExecuteCommand("Cofoundry.FailedAuthticationAttempt_Add",
                new SqlParameter("UserAreaCode", command.UserAreaCode),
                new SqlParameter("Username", TextFormatter.Limit(command.Username, 150)),
                new SqlParameter("IPAddress", connectionInfo.IPAddress),
                new SqlParameter("DateTimeNow", executionContext.ExecutionDate)
                );
        }

        public async Task ExecuteAsync(LogFailedLoginAttemptCommand command, IExecutionContext executionContext)
        {
            var connectionInfo = _clientConnectionService.GetConnectionInfo();

            await _sqlExecutor.ExecuteCommandAsync("Cofoundry.FailedAuthticationAttempt_Add",
                new SqlParameter("UserAreaCode", command.UserAreaCode),
                new SqlParameter("Username", TextFormatter.Limit(command.Username, 150)),
                new SqlParameter("IPAddress", connectionInfo.IPAddress),
                new SqlParameter("DateTimeNow", executionContext.ExecutionDate)
                );
        }
    }
}
