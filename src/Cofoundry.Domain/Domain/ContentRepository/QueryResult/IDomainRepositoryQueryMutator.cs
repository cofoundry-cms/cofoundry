using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Wraps a repository query with mutator actions
    /// that allow you to change the result after execution.
    /// </summary>
    /// <typeparam name="TQueryResult">The type of the original query result.</typeparam>
    /// <typeparam name="TOutput">The result type after the mutation has been applied.</typeparam>
    public interface IDomainRepositoryQueryMutator<TQueryResult, TOutput>
    {
        /// <summary>
        /// The original query that is to be executed prior to mutation.
        /// </summary>
        IQuery<TQueryResult> Query { get; }

        /// <summary>
        /// Executes the query, applies any mutators and returns the result.
        /// </summary>
        Task<TOutput> ExecuteAsync();
    }
}
