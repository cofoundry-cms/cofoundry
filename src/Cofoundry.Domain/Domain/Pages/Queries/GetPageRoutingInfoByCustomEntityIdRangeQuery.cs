using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetPageRoutingInfoByCustomEntityIdRangeQuery : IQuery<IDictionary<int, ICollection<PageRoutingInfo>>>
    {
        public GetPageRoutingInfoByCustomEntityIdRangeQuery() { }

        public GetPageRoutingInfoByCustomEntityIdRangeQuery(IEnumerable<int> customEntityIds)
            : this(customEntityIds?.ToList())
        {
        }

        public GetPageRoutingInfoByCustomEntityIdRangeQuery(IReadOnlyCollection<int> customEntityIds)
        {
            CustomEntityIds = customEntityIds;
        }

        public IReadOnlyCollection<int> CustomEntityIds { get; set; }
    }
}
