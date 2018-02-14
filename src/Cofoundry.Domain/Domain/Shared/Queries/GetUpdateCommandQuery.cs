using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.CQS
{
    /// <summary>
    /// A query to get a single entity without .
    /// </summary>
    /// <typeparam name="TEntity">type of entity to return</typeparam>
    public class GetUpdateCommandQuery<TEntity> 
        : IQuery<TEntity>
        where TEntity : ICommand
    {
    }
}
