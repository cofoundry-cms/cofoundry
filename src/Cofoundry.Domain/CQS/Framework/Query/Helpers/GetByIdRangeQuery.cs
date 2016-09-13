using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.CQS
{
    /// <summary>
    /// A query to get a single entity of the specified type using an integer identifier.
    /// </summary>
    /// <typeparam name="TEntity">type of entity to return</typeparam>
    public class GetByIdRangeQuery<TEntity> : IQuery<IDictionary<int, TEntity>>
    {
        public GetByIdRangeQuery()
        {
        }

        public GetByIdRangeQuery(IEnumerable<int> ids)
        {
            Ids = ids.ToArray();
        }

        public int[] Ids { get; set; }
    }
}
