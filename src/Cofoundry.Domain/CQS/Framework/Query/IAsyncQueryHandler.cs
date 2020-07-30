using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.CQS
{
    /// <summary>
    /// A handler that is used to execute an instance of IQuery.
    /// </summary>
    /// <typeparam name="TQuery">Type of IQuery object to execute</typeparam>
    /// <typeparam name="TResult">The type of the result to be returned from the query</typeparam>
    [Obsolete("Use the renamed interface IQueryHandler instead.")]
    public interface IAsyncQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        /// <summary>
        /// Executes the specified query using the specified ExecutionContext.
        /// </summary>
        /// <param name="query">IQuery object to execute</param>
        /// <param name="executionContext">The context to execute the query under, i.e. which user what execution timestamp</param>
        /// <returns>The results of the query.</returns>
        Task<TResult> ExecuteAsync(TQuery query, IExecutionContext executionContext);
    }
}
