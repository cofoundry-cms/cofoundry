using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetEntityDependencySummaryByRelatedEntityQuery : IQuery<ICollection<EntityDependencySummary>>
    {
        public GetEntityDependencySummaryByRelatedEntityQuery()
        {
        }

        public GetEntityDependencySummaryByRelatedEntityQuery(string entityDefinitionCode, int entityId)
        {
            EntityDefinitionCode = entityDefinitionCode;
            EntityId = entityId;
        }

        [Required]
        public string EntityDefinitionCode { get; set; }

        public int EntityId { get; set; }
    }
}
