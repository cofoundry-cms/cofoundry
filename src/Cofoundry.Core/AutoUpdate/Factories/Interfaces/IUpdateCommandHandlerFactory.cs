using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.AutoUpdate
{
    /// <summary>
    /// Factory that creates concrete implementations of IUpdateCommand handlers.
    /// </summary>
    public interface IUpdateCommandHandlerFactory
    {
        IUpdateCommandHandler<TCommand> Create<TCommand>() where TCommand : IUpdateCommand;
    }
}
