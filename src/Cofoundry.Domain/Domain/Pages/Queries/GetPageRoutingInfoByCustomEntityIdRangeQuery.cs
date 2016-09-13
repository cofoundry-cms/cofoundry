using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public GetPageRoutingInfoByCustomEntityIdRangeQuery(string customEntityDefinitionCode, IEnumerable<int> ids)
        {
            Ids = ids.ToArray();
            CustomEntityDefinitionCode = customEntityDefinitionCode;
        }

        [Required]
        public string CustomEntityDefinitionCode { get; set; }
    }
}
