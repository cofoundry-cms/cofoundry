using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A single repository for all Cofoundry queries and
    /// commands to make them easier to discover.
    /// </summary>
    public interface IAdvancedContentRepository
    {
        /// <summary>
        /// Directly executes an IQuery instance and returns
        /// the results.
        /// </summary>
        /// <param name="query">Query to execute.</param>
        Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query);

        /// <summary>
        /// Directly executes an ICommand instance.
        /// </summary>
        /// <param name="command">Command to execute.</param>
        Task ExecuteCommandAsync(ICommand command);
    }
}
