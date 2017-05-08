using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.CQS
{
    /// <summary>
    /// Handles the execution IQuery instances.
    /// </summary>
    public interface IQueryExecutor
    {
        /// <summary>
        /// Handles the asynchronous execution the specified query.
        /// </summary>
        /// <param name="query">Query to execute.</param>
        Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query);

        /// <summary>
        /// Handles the asynchronous execution the specified query.
        /// </summary>
        /// <param name="query">Query to execute.</param>
        /// <param name="executionContext">
        /// Optional custom execution context which can be used to impersonate/elevate permissions 
        /// or change the execution date.
        /// </param>
        Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, IExecutionContext executionContext = null);
    }
}
