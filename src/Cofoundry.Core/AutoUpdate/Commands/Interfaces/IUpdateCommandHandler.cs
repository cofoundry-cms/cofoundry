using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.AutoUpdate
{
    /// <summary>
    /// Base interface for IUpdateCommands handlers
    /// </summary>
    /// <typeparam name="TCommand">Type of command to execute</typeparam>
    public interface IUpdateCommandHandler<in TCommand> where TCommand : IUpdateCommand
    {
    }
}
