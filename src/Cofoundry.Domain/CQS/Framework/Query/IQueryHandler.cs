using System.Threading.Tasks;

namespace Cofoundry.Domain.CQS
{
    /// <summary>
    /// Handles the execution of a <see cref="TQuery"/> instance.
    /// </summary>
    /// <typeparam name="TQuery">Type of IQuery object to execute.</typeparam>
    /// <typeparam name="TResult">The result type to be returned from the query.</typeparam>
    public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        /// <summary>
        /// Executes the specified query using the specified <see cref="IExecutionContext"/>.
        /// </summary>
        /// <param name="query"><see cref="IQuery"/> instance containing the parameters of the query.</param>
        /// <param name="executionContext">The context to execute the query under, i.e. which user and execution timestamp etc.</param>
        /// <returns>The results of the query.</returns>
        Task<TResult> ExecuteAsync(TQuery query, IExecutionContext executionContext);

    }
}
