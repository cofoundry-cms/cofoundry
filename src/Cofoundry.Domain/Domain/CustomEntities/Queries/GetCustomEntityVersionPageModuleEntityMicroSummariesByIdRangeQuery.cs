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
    public class GetCustomEntityVersionPageModuleEntityMicroSummariesByIdRangeQuery : IQuery<IDictionary<int, RootEntityMicroSummary>>
    {
        public GetCustomEntityVersionPageModuleEntityMicroSummariesByIdRangeQuery ()
        {
        }

        public GetCustomEntityVersionPageModuleEntityMicroSummariesByIdRangeQuery(
            IEnumerable<int> ids
            )
        {
            Condition.Requires(ids).IsNotNull();

            CustomEntityVersionPageModuleIds = ids.ToArray();
        }

        [Required]
        public int[] CustomEntityVersionPageModuleIds { get; set; }
    }
}
