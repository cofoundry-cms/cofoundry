using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetPageVersionEntityMicroSummariesByIdRangeQuery : IQuery<IDictionary<int, RootEntityMicroSummary>>
    {
        public GetPageVersionEntityMicroSummariesByIdRangeQuery()
        {
        }

        public GetPageVersionEntityMicroSummariesByIdRangeQuery(IEnumerable<int> pageVersionIds)
            : this(pageVersionIds.ToList())
        {
        }

        public GetPageVersionEntityMicroSummariesByIdRangeQuery(IReadOnlyCollection<int> pageVersionIds)
        {
            if (pageVersionIds == null) throw new ArgumentNullException(nameof(pageVersionIds));

            PageVersionIds = pageVersionIds;
        }

        [Required]
        public IReadOnlyCollection<int> PageVersionIds { get; set; }
    }
}
