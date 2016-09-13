using Conditions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

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
            Condition.Requires(ids).IsNotNull();

            CustomEntityVersionIds = ids.ToArray();
        }

        [Required]
        public int[] CustomEntityVersionIds { get; set; }
    }
}
