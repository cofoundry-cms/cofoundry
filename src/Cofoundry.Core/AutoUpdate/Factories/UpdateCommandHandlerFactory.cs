using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Core.AutoUpdate
{
    /// <summary>
    /// Factory that creates concrete implementations of IUpdateCommand handlers.
    /// </summary>
    public class UpdateCommandHandlerFactory : IUpdateCommandHandlerFactory
    {
        private readonly IResolutionContext _resolutionContext;

        public UpdateCommandHandlerFactory(
            IResolutionContext resolutionContext
            )
        {
            _resolutionContext = resolutionContext;
        }

        public IUpdateCommandHandler<TCommand> Create<TCommand>() where TCommand : IUpdateCommand
        {
            if (_resolutionContext.IsRegistered<IAsyncUpdateCommandHandler<TCommand>>())
            {
                return _resolutionContext.Resolve<IAsyncUpdateCommandHandler<TCommand>>();
            }
            else
            {
                return _resolutionContext.Resolve<ISyncUpdateCommandHandler<TCommand>>();
            }
        }
    }
}
