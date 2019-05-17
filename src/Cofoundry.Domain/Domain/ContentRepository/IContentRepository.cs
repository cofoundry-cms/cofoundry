using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple access to Cofoundry domain queries and commands from 
    /// a single repository. A more extensive range of queries and
    /// commands are available in IAdvancedContentRepository.
    /// </summary>
    public interface IContentRepository
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
