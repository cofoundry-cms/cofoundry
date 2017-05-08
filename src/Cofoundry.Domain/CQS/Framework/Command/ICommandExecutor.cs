using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.CQS
{
    /// <summary>
    /// Service for executing commands of various types.
    /// </summary>
    /// <remarks>
    /// See http://www.cuttingedge.it/blogs/steven/pivot/entry.php?id=91
    /// </remarks>
    public interface ICommandExecutor
    {
        /// <summary>
        /// Handles the execution the specified command.
        /// </summary>
        /// <param name="command">Command to execute.</param>
        Task ExecuteAsync(ICommand command);

        /// <summary>
        /// Handles the execution the specified command.
        /// </summary>
        /// <param name="command">Command to execute.</param>
        /// <param name="executionContext">
        /// Optional custom execution context which can be used to impersonate/elevate permissions 
        /// or change the execution date.
        /// </param>
        Task ExecuteAsync(ICommand command, IExecutionContext executionContext = null);
    }
}
