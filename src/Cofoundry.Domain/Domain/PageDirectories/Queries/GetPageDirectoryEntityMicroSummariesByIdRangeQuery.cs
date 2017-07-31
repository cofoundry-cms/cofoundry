using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetPageDirectoryEntityMicroSummariesByIdRangeQuery : IQuery<IDictionary<int, RootEntityMicroSummary>>
    {
        public GetPageDirectoryEntityMicroSummariesByIdRangeQuery()
        {
        }

        public GetPageDirectoryEntityMicroSummariesByIdRangeQuery(
            IEnumerable<int> ids
            )
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));

            PageDirectoryIds = ids.ToArray();
        }

        [Required]
        public int[] PageDirectoryIds { get; set; }
    }
}
