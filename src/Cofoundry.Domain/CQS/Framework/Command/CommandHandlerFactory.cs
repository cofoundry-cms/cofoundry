using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Domain.CQS
{
    /// <summary>
    /// Factory to create ICommandHandler instances
    /// </summary>
    public class CommandHandlerFactory : ICommandHandlerFactory
    {
        private readonly IResolutionContext _resolutionContext;

        public CommandHandlerFactory(
            IResolutionContext resolutionContext
            )
        {
            _resolutionContext = resolutionContext;
        }

        /// <summary>
        /// Creates a new ICommandHandler instance with the specified type signature.
        /// </summary>
        public ICommandHandler<T> Create<T>() where T : ICommand
        {
            return _resolutionContext.Resolve<ICommandHandler<T>>();
        }

        /// <summary>
        /// Creates a new IAsyncCommandHandler instance with the specified type signature.
        /// </summary>
        public IAsyncCommandHandler<T> CreateAsyncHandler<T>() where T : ICommand
        {
            return _resolutionContext.Resolve<IAsyncCommandHandler<T>>();
        }
    }
}
