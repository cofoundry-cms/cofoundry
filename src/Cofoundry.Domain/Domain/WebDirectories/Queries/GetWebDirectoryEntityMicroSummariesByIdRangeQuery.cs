using Conditions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetWebDirectoryEntityMicroSummariesByIdRangeQuery : IQuery<IDictionary<int, RootEntityMicroSummary>>
    {
        public GetWebDirectoryEntityMicroSummariesByIdRangeQuery()
        {
        }

        public GetWebDirectoryEntityMicroSummariesByIdRangeQuery(
            IEnumerable<int> ids
            )
        {
            Condition.Requires(ids).IsNotNull();

            WebDirectoryIds = ids.ToArray();
        }

        [Required]
        public int[] WebDirectoryIds { get; set; }
    }
}
