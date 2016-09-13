using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.CQS
{
    /// <summary>
    /// Factory to create IQueryHandler instances. This factory allows you to override
    /// or wrap the existing IQueryHandler implementation
    /// </summary>
    public interface IQueryHandlerFactory
    {
        /// <summary>
        /// Creates a new IQueryHandler instance with the specified type signature.
        /// </summary>
        IQueryHandler<TQuery, TResult> Create<TQuery, TResult>() where TQuery : IQuery<TResult>;

        /// <summary>
        /// Creates a new IAsyncQueryHandler instance with the specified type signature.
        /// </summary>
        IAsyncQueryHandler<TQuery, TResult> CreateAsyncHandler<TQuery, TResult>() where TQuery : IQuery<TResult>;
    }
}
