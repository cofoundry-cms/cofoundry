using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetCustomEntityVersionEntityMicroSummariesByIdRangeQuery : IQuery<IDictionary<int, RootEntityMicroSummary>>
    {
        public GetCustomEntityVersionEntityMicroSummariesByIdRangeQuery()
        {
        }

        public GetCustomEntityVersionEntityMicroSummariesByIdRangeQuery(
            IEnumerable<int> ids
            )
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));

            CustomEntityVersionIds = ids.ToArray();
        }

        [Required]
        public int[] CustomEntityVersionIds { get; set; }
    }
}
