using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.CQS
{
    /// <summary>
    /// A handler that is used to execute an instance of IQuery.
    /// </summary>
    /// <typeparam name="TQuery">Type of IQuery object to execute</typeparam>
    /// <typeparam name="TResult">The type of the result to be returned from the query</typeparam>
    public interface IQueryHandler<TQuery, TResult> : IAsyncQueryHandler<TQuery, TResult>
         where TQuery : IQuery<TResult>
    {
    }
}
