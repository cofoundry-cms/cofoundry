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

        public GetPageVersionBlockEntityMicroSummariesByIdRangeQuery(IEnumerable<int> pageVersionBlockIds)
            : this(pageVersionBlockIds?.ToList())
        {
        }

        public GetPageVersionBlockEntityMicroSummariesByIdRangeQuery(IReadOnlyCollection<int> pageVersionBlockIds)
        {
            if (pageVersionBlockIds == null) throw new ArgumentNullException(nameof(pageVersionBlockIds));

            PageVersionBlockIds = pageVersionBlockIds;
        }

        [Required]
        public IReadOnlyCollection<int> PageVersionBlockIds { get; set; }
    }
}
