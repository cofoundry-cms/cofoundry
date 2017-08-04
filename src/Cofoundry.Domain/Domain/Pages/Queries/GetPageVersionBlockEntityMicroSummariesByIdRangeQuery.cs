using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetPageVersionBlockEntityMicroSummariesByIdRangeQuery : IQuery<IDictionary<int, RootEntityMicroSummary>>
    {
        public GetPageVersionBlockEntityMicroSummariesByIdRangeQuery()
        {
        }

        public GetPageVersionBlockEntityMicroSummariesByIdRangeQuery(
            IEnumerable<int> ids
            )
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));

            PageVersionBlockIds = ids.ToArray();
        }

        [Required]
        public int[] PageVersionBlockIds { get; set; }
    }
}
