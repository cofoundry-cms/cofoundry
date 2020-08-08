using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.CQS.Internal
{
    /// <summary>
    /// Basic service for logging audit data about executed commands.
    /// </summary>
    public class DebugCommandLogService : ICommandLogService
    {
        private readonly ILogger<DebugCommandLogService> _logger;

        public DebugCommandLogService(
            ILogger<DebugCommandLogService>  logger
            )
        {
            _logger = logger;
        }

        public Task LogAsync<TCommand>(TCommand command, IExecutionContext executionContext) where TCommand : ICommand
        {
            _logger.LogInformation(
                "{ExecutionDate} SUCCESS executing command {CommandName}. User {UserId}",
                executionContext.ExecutionDate,
                typeof(TCommand).FullName,
                executionContext.UserContext.UserId?.ToString()
                );

            return Task.CompletedTask;
        }

        public Task LogFailedAsync<TCommand>(TCommand command, IExecutionContext executionContext, Exception ex = null) where TCommand : ICommand
        {
            if (command is ILoggableCommand)
            {
                // Exception should be picked up by and handled elsewhere, so just log information.

                _logger.LogInformation(
                    "{ExecutionDate} FAILED executing command {CommandName}. User {UserId}, Exception '{ExceptionMessage}'",
                    executionContext.ExecutionDate,
                    typeof(TCommand).FullName,
                    executionContext.UserContext.UserId?.ToString(),
                    ex?.Message
                    );
            }

            return Task.CompletedTask;
        }
    }
}
