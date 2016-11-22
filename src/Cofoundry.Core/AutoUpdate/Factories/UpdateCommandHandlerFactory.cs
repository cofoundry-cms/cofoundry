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

        public IVersionedUpdateCommandHandler<TCommand> CreateVersionedCommand<TCommand>() where TCommand : IVersionedUpdateCommand
        {
            if (_resolutionContext.IsRegistered<IAsyncVersionedUpdateCommandHandler<TCommand>>())
            {
                return _resolutionContext.Resolve<IAsyncVersionedUpdateCommandHandler<TCommand>>();
            }
            else
            {
                return _resolutionContext.Resolve<ISyncVersionedUpdateCommandHandler<TCommand>>();
            }
        }

        public IAlwaysRunUpdateCommandHandler<TCommand> CreateAlwaysRunCommand<TCommand>() where TCommand : IAlwaysRunUpdateCommand
        {
            if (_resolutionContext.IsRegistered<IAsyncAlwaysRunUpdateCommandHandler<TCommand>>())
            {
                return _resolutionContext.Resolve<IAsyncAlwaysRunUpdateCommandHandler<TCommand>>();
            }
            else
            {
                return _resolutionContext.Resolve<ISyncAlwaysRunUpdateCommandHandler<TCommand>>();
            }
        }
    }
}
