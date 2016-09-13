using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.AutoUpdate
{
    /// <summary>
    /// Handler for executing IUpdateCommands synchronously
    /// </summary>
    /// <typeparam name="TCommand">Type of command to execute</typeparam>
    public interface ISyncUpdateCommandHandler<in TCommand> : IUpdateCommandHandler<TCommand>
        where TCommand : IUpdateCommand
    {
        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <param name="command">Command to execute</param>
        void Execute(TCommand command);
    }
}
