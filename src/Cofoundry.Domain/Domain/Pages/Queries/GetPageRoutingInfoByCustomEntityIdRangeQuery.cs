using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetPageRoutingInfoByCustomEntityIdRangeQuery : GetByIdRangeQuery<IEnumerable<PageRoutingInfo>>
    {
        public GetPageRoutingInfoByCustomEntityIdRangeQuery()
        {
        }

        public GetPageRoutingInfoByCustomEntityIdRangeQuery(IEnumerable<int> ids)
        {
            Ids = ids.ToArray();
        }
    }
}
