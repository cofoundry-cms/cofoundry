using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.CQS
{
    /// <summary>
    /// Basic service for logging audit data about executed commands.
    /// </summary>
    public class DebugCommandLogService : ICommandLogService
    {
        public void Log<TCommand>(TCommand command, IExecutionContext executionContext) where TCommand : ICommand
        {
            if (command is ILoggableCommand)
            {
                var msg = string.Format("{0} SUCCESS executing command {1}. User {2}",
                executionContext.ExecutionDate,
                typeof(TCommand).FullName,
                executionContext.UserContext.UserId?.ToString()
                );
                Debug.WriteLine(msg);
            }
        }

        public Task LogAsync<TCommand>(TCommand command, IExecutionContext executionContext) where TCommand : ICommand
        {
            Log(command, executionContext);
            return Task.FromResult(true);
        }

        public void LogFailed<TCommand>(TCommand command, IExecutionContext executionContext, Exception ex = null) where TCommand : ICommand
        {
            if (command is ILoggableCommand)
            {
                var msg = string.Format("{0} FAILED executing command {1}. User {2}, Exception '{3}'",
                executionContext.ExecutionDate,
                typeof(TCommand).FullName,
                executionContext.UserContext.UserId?.ToString(),
                ex?.Message
                );
                Debug.WriteLine(msg);
            }
        }

        public Task LogFailedAsync<TCommand>(TCommand command, IExecutionContext executionContext, Exception ex = null) where TCommand : ICommand
        {
            LogFailedAsync(command, executionContext);
            return Task.FromResult(true);
        }
    }
}
