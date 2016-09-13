using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.CQS
{
    /// <summary>
    /// A query to get all entities of a particular type. 
    /// </summary>
    /// <typeparam name="TEntity">type of entity to return</typeparam>
    public class GetAllQuery<TEntity> : IQuery<IEnumerable<TEntity>>
    {
    }
}
