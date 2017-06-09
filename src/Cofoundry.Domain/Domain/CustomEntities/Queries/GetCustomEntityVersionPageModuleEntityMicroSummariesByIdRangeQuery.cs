using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

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
            if (ids == null) throw new ArgumentNullException(nameof(ids));

            CustomEntityVersionPageModuleIds = ids.ToArray();
        }

        [Required]
        public int[] CustomEntityVersionPageModuleIds { get; set; }
    }
}
