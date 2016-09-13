using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.CQS
{
    /// <summary>
    /// A query to get a single entity of the specified type using a string identifier.
    /// </summary>
    /// <typeparam name="TEntity">type of entity to return</typeparam>
    public class GetByStringQuery<TEntity> : IQuery<TEntity>
    {
        public GetByStringQuery()
        {
        }

        public GetByStringQuery(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}
