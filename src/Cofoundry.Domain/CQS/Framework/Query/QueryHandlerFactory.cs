using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Cofoundry.Domain.CQS.Internal
{
    /// <summary>
    /// Factory to create the default QueryHandler instance
    /// </summary>
    public class QueryHandlerFactory : IQueryHandlerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public QueryHandlerFactory(
            IServiceProvider serviceProvider
            )
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Creates a new IAsyncQueryHandler instance with the specified type signature.
        /// </summary>
        public IAsyncQueryHandler<TQuery, TResult> CreateAsyncHandler<TQuery, TResult>() where TQuery : IQuery<TResult>
        {
            return _serviceProvider.GetRequiredService<IAsyncQueryHandler<TQuery, TResult>>();
        }
    }
}
