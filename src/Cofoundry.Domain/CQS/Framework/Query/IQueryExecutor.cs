using System.Threading.Tasks;

namespace Cofoundry.Domain.CQS
{
    /// <summary>
    /// Handles the execution IQuery instances.
    /// </summary>
    public interface IQueryExecutor
    {
        /// <summary>
        /// Handles the execution the specified <paramref name="query"/>.
        /// </summary>
        /// <param name="query">Query to execute.</param>
        Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query);

        /// <summary>
        /// Handles the execution the specified <paramref name="query"/>.
        /// </summary>
        /// <param name="query">Query to execute.</param>
        /// <param name="executionContext">
        /// Optional custom execution context which can be used to impersonate/elevate permissions 
        /// or change the execution date.
        /// </param>
        Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, IExecutionContext executionContext);

        /// <summary>
        /// Handles the execution the specified <paramref name="query"/>.
        /// </summary>
        /// <param name="query">Query to execute.</param>
        /// <param name="userContext">
        /// Optional user context which can be used to impersonate/elevate permissions.
        /// </param>
        Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, IUserContext userContext);
    }
}
