using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetCustomEntityRenderSummariesByIdRangeQuery : IQuery<IDictionary<int, CustomEntityRenderSummary>>
    {
        public GetCustomEntityRenderSummariesByIdRangeQuery()
        {
        }

        public GetCustomEntityRenderSummariesByIdRangeQuery(
            IEnumerable<int> customEntityIds,
            PublishStatusQuery publishStatusQuery = PublishStatusQuery.Published
            )
            : this(customEntityIds?.ToList(), publishStatusQuery)
        {
        }

        public GetCustomEntityRenderSummariesByIdRangeQuery(
            IReadOnlyCollection<int> customEntityIds,
            PublishStatusQuery publishStatusQuery = PublishStatusQuery.Published
            )
        {
            if (customEntityIds == null) throw new ArgumentNullException(nameof(customEntityIds));

            CustomEntityIds = customEntityIds;
            PublishStatus = publishStatusQuery;
        }

        [Required]
        public IReadOnlyCollection<int> CustomEntityIds { get; set; }

        public PublishStatusQuery PublishStatus { get; set; }
    }
}
