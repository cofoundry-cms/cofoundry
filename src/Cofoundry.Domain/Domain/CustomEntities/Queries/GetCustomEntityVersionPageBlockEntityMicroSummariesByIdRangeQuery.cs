using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetCustomEntityVersionPageBlockEntityMicroSummariesByIdRangeQuery : IQuery<IDictionary<int, RootEntityMicroSummary>>
    {
        public GetCustomEntityVersionPageBlockEntityMicroSummariesByIdRangeQuery ()
        {
        }

        public GetCustomEntityVersionPageBlockEntityMicroSummariesByIdRangeQuery(
            IEnumerable<int> ids
            )
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));

            CustomEntityVersionPageBlockIds = ids.ToArray();
        }

        [Required]
        public int[] CustomEntityVersionPageBlockIds { get; set; }
    }
}
