using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetCustomEntitySummariesByIdRangeQuery : IQuery<IDictionary<int, CustomEntitySummary>>
    {
        public GetCustomEntitySummariesByIdRangeQuery()
        {
        }

        public GetCustomEntitySummariesByIdRangeQuery(
            IEnumerable<int> customEntityIds,
            PublishStatusQuery publishStatusQuery = PublishStatusQuery.Published
            )
        {
            if (customEntityIds == null) throw new ArgumentNullException(nameof(customEntityIds));

            CustomEntityIds = customEntityIds.ToList();
            PublishStatus = publishStatusQuery;
        }

        public GetCustomEntitySummariesByIdRangeQuery(
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
