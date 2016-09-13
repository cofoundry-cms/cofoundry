using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetPageRoutingInfoByCustomEntityIdQuery : IQuery<IEnumerable<PageRoutingInfo>>
    {
        public GetPageRoutingInfoByCustomEntityIdQuery()
        {
        }

        public GetPageRoutingInfoByCustomEntityIdQuery(
            string customEntityDefinitionCode, 
            int customEntityId
            )
        {
            CustomEntityDefinitionCode = customEntityDefinitionCode;
            CustomEntityId = customEntityId;
        }

        public int CustomEntityId { get; set; }

        [Required]
        public string CustomEntityDefinitionCode { get; set; }
    }
}
