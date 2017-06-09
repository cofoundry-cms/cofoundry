using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetCustomEntityEntityMicroSummariesByIdRangeQuery : IQuery<IDictionary<int, RootEntityMicroSummary>>
    {
        public GetCustomEntityEntityMicroSummariesByIdRangeQuery()
        {
        }

        public GetCustomEntityEntityMicroSummariesByIdRangeQuery(
            IEnumerable<int> customEntityIds
            )
        {
            if (customEntityIds == null) throw new ArgumentNullException(nameof(customEntityIds));

            CustomEntityIds = customEntityIds.ToArray();
        }

        [Required]
        public int[] CustomEntityIds { get; set; }
    }
}
