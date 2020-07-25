using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents a content repository query that is ready to execute.
    /// </summary>
    /// <typeparam name="TQueryResult">The type of the query result.</typeparam>
    public interface IContentRepositoryQueryContext<TQueryResult>
        : IContentRepositoryQueryMutator<TQueryResult, TQueryResult>
    {
    }
}
