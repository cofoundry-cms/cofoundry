using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetPageVersionModuleEntityMicroSummariesByIdRangeQuery : IQuery<IDictionary<int, RootEntityMicroSummary>>
    {
        public GetPageVersionModuleEntityMicroSummariesByIdRangeQuery()
        {
        }

        public GetPageVersionModuleEntityMicroSummariesByIdRangeQuery(
            IEnumerable<int> ids
            )
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));

            PageVersionModuleIds = ids.ToArray();
        }

        [Required]
        public int[] PageVersionModuleIds { get; set; }
    }
}
