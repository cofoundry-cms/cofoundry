using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Domain.CQS
{
    /// <summary>
    /// Factory to create the default QueryHandler instance
    /// </summary>
    public class QueryHandlerFactory : IQueryHandlerFactory
    {
        private readonly IResolutionContext _resolutionContext;

        public QueryHandlerFactory(
            IResolutionContext resolutionContext
            )
        {
            _resolutionContext = resolutionContext;
        }

        /// <summary>
        /// Creates a new IQueryHandler instance with the specified type signature.
        /// </summary>
        public IQueryHandler<TQuery, TResult> Create<TQuery, TResult>() where TQuery : IQuery<TResult>
        {
            return _resolutionContext.Resolve<IQueryHandler<TQuery, TResult>>();
        }

        /// <summary>
        /// Creates a new IAsyncQueryHandler instance with the specified type signature.
        /// </summary>
        public IAsyncQueryHandler<TQuery, TResult> CreateAsyncHandler<TQuery, TResult>() where TQuery : IQuery<TResult>
        {
            return _resolutionContext.Resolve<IAsyncQueryHandler<TQuery, TResult>>();
        }
    }
}
