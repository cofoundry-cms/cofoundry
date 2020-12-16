using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.CQS
{
    /// <summary>
    /// Handles asynchronous execution of an ICommand.
    /// </summary>
    /// <typeparam name="TCommand">Type of Command to handle</typeparam>
    [Obsolete("Use the renamed interface ICommandHandler instead.")]
    public interface IAsyncCommandHandler<TCommand> where TCommand : ICommand
    {
        Task ExecuteAsync(TCommand command, IExecutionContext executionContext);
    }
}
