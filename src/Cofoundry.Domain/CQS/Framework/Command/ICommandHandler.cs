using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.CQS
{
    /// <summary>
    /// Handles asynchronous execution of an ICommand.
    /// </summary>
    /// <typeparam name="TCommand">Type of Command to handle</typeparam>
    public interface ICommandHandler<TCommand> : IAsyncCommandHandler<TCommand>
         where TCommand : ICommand
    {
    }
}
