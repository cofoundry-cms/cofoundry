using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetPageRoutingInfoByCustomEntityIdQuery : IQuery<ICollection<PageRoutingInfo>>
    {
        public GetPageRoutingInfoByCustomEntityIdQuery()
        {
        }

        public GetPageRoutingInfoByCustomEntityIdQuery(
            int customEntityId
            )
        {
            CustomEntityId = customEntityId;
        }

        public int CustomEntityId { get; set; }
    }
}
