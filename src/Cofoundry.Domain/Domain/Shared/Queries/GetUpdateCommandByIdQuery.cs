using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.CQS
{
    /// <summary>
    /// A query to get a single entity of the specified type using an integer identifier.
    /// </summary>
    /// <typeparam name="TEntity">type of entity to return</typeparam>
    public class GetUpdateCommandByIdQuery<TEntity> 
        : IQuery<TEntity>
        where TEntity : ICommand
    {
        public GetUpdateCommandByIdQuery()
        {
        }

        public GetUpdateCommandByIdQuery(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
    }
}
