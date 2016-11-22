using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.AutoUpdate
{
    /// <summary>
    /// Base interface for IVersionedUpdateCommand handlers
    /// </summary>
    /// <typeparam name="TCommand">Type of command to execute</typeparam>
    public interface IVersionedUpdateCommandHandler<in TCommand> where TCommand : IVersionedUpdateCommand
    {
    }
}
