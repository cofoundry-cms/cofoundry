using Microsoft.Extensions.DependencyInjection;
using System;

namespace Cofoundry.Domain.CQS.Internal
{
    /// <summary>
    /// Factory to create ICommandHandler instances
    /// </summary>
    public class CommandHandlerFactory : ICommandHandlerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandHandlerFactory(
            IServiceProvider serviceProvider
            )
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Creates a new IAsyncCommandHandler instance with the specified type signature.
        /// </summary>
        public ICommandHandler<T> CreateAsyncHandler<T>() where T : ICommand
        {
            return _serviceProvider.GetRequiredService<ICommandHandler<T>>();
        }
    }
}
