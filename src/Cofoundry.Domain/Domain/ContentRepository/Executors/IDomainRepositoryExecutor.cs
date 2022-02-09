using Cofoundry.Domain.CQS;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Extendable
{
    /// <summary>
    /// Manages the execution of queries and commands in a domain repository
    /// instance. This is separate from the repository so it can be switched 
    /// out or decorated with different behaviors.
    /// </summary>
    public interface IDomainRepositoryExecutor
    {
        /// <summary>
        /// Handles the execution of the specified <paramref name="command"/>.
        /// </summary>
        /// <param name="command">Command to execute.</param>
        /// <param name="executionContext">
        /// Optional custom execution context which can be used to impersonate/elevate permissions 
        /// or change the execution date.
        /// </param>
        Task ExecuteAsync(ICommand command, IExecutionContext executionContext);

        /// <summary>
        /// Handles the execution the specified <paramref name="query"/>.
        /// </summary>
        /// <param name="query">Query to execute.</param>
        /// <param name="executionContext">
        /// Optional custom execution context which can be used to impersonate/elevate permissions 
        /// or change the execution date.
        /// </param>
        Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, IExecutionContext executionContext);
    }
}