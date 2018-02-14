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
        public GetPageDirectoryEntityMicroSummariesByIdRangeQuery() { }

        public GetPageDirectoryEntityMicroSummariesByIdRangeQuery(IEnumerable<int> ids)
            : this (ids.ToList())
        {
        }

        public GetPageDirectoryEntityMicroSummariesByIdRangeQuery(IReadOnlyCollection<int> ids)
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));

            PageDirectoryIds = ids;
        }

        [Required]
        public IReadOnlyCollection<int> PageDirectoryIds { get; set; }
    }
}
