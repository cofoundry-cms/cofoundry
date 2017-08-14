using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.CQS
{
    /// <summary>
    /// Service to validate the permissions of command/query handler prior to execution.
    /// </summary>
    public interface IExecutePermissionValidationService
    {
        void Validate<TCommand>(TCommand command, IAsyncCommandHandler<TCommand> commandHandler, IExecutionContext executionContext) where TCommand : ICommand;

        void Validate<TQuery, TResult>(TQuery query, IAsyncQueryHandler<TQuery, TResult> queryHandler, IExecutionContext executionContext) where TQuery : IQuery<TResult>;
    }
}
