using Microsoft.Extensions.DependencyInjection;
using System;

namespace Cofoundry.Core.Mail.Internal
{
    /// <inheritdoc/>
    public class DefaultMailDispatchSessionFactory : IMailDispatchSessionFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultMailDispatchSessionFactory(
            IServiceProvider serviceProvider
            )
        {
            _serviceProvider = serviceProvider;
        }

        public IMailDispatchSession Create()
        {
            // By default, use the DI container to resolve the session
            // which under normal circumstances should be registered as transient
            // This allows the session implementation to be swapped out independently
            // which is a little more convenient than creating a whole new IMailDispatchService 
            // implementation
            return _serviceProvider.GetRequiredService<IMailDispatchSession>();
        }
    }
}
