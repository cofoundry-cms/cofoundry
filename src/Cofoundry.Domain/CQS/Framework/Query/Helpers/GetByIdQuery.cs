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
    public class GetByIdQuery<TEntity> : IQuery<TEntity>
    {
        public GetByIdQuery()
        {
        }

        public GetByIdQuery(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
    }
}
