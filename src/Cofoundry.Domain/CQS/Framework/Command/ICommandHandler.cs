using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.CQS
{
    /// <summary>
    /// Handles the execution of an ICommand.
    /// </summary>
    /// <typeparam name="TCommand">Type of Command to handle</typeparam>
    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        void Execute(TCommand command, IExecutionContext executionContext);
    }
}
