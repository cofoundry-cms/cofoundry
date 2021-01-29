using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

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
        public IAsyncCommandHandler<T> CreateAsyncHandler<T>() where T : ICommand
        {
            return _serviceProvider.GetRequiredService<IAsyncCommandHandler<T>>();
        }
    }
}
