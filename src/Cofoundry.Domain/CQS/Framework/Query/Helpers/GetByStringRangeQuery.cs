using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.CQS
{
    /// <summary>
    /// A query to get a set of entities of the specified type using a range of string identifiers.
    /// </summary>
    /// <typeparam name="TEntity">type of entity to return</typeparam>
    public class GetByStringRangeQuery<TEntity> : IQuery<IDictionary<string, TEntity>>
    {
        public GetByStringRangeQuery()
        {
        }

        public GetByStringRangeQuery(IEnumerable<string> ids)
        {
            Ids = ids.ToArray();
        }

        public string[] Ids { get; set; }
    }
}
