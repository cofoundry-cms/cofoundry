using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.CQS
{
    /// <summary>
    /// Represents query parameters that when executed will yield an
    /// instance of TResult.
    /// </summary>
    /// <remarks>
    /// See http://www.cuttingedge.it/blogs/steven/pivot/entry.php?id=92
    /// </remarks>
    /// <typeparam name="TResult">The type of result to returned from the query</typeparam>
    public interface IQuery<TResult>
    {
    }
}
